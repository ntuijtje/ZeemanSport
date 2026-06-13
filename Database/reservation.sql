CREATE TABLE IF NOT EXISTS dbo.reservations
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    session_id integer NOT NULL REFERENCES dbo.sessions(id) ON DELETE CASCADE,
    user_id integer NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE,
    status integer NOT NULL DEFAULT 1,
    seat_row integer NULL,
    seat_column integer NULL,
    reserved_at timestamp NOT NULL DEFAULT current_timestamp,
    checked_in_at timestamp NULL,
    check_in_method integer NULL
);

-- A member can only hold one active (non-cancelled) reservation per session, and within a session
-- a specific bike can only be taken once.
CREATE UNIQUE INDEX IF NOT EXISTS ux_reservations_active_user
    ON dbo.reservations(session_id, user_id) WHERE status <> 2;
CREATE UNIQUE INDEX IF NOT EXISTS ux_reservations_active_seat
    ON dbo.reservations(session_id, seat_row, seat_column) WHERE status <> 2 AND seat_row IS NOT NULL;

CREATE TABLE IF NOT EXISTS dbo.waitlist_entries
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    session_id integer NOT NULL REFERENCES dbo.sessions(id) ON DELETE CASCADE,
    user_id integer NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE,
    created_at timestamp NOT NULL DEFAULT current_timestamp,
    UNIQUE (session_id, user_id)
);

-- Reservations enriched with their session details. The p_* arguments filter the same base query
-- so the C# repository always maps an identical column set.
CREATE OR REPLACE FUNCTION dbo.usp_get_reservation_rows(p_id integer, p_user_id integer, p_session_id integer, p_active_only boolean, p_from timestamp, p_to timestamp)
RETURNS TABLE
(
    id integer,
    session_id integer,
    user_id integer,
    status integer,
    seat_row integer,
    seat_column integer,
    reserved_at timestamp,
    checked_in_at timestamp,
    check_in_method integer,
    workout_name varchar,
    start_time timestamp,
    duration_minutes integer,
    location_name varchar,
    location_type integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        r.id,
        r.session_id,
        r.user_id,
        r.status,
        r.seat_row,
        r.seat_column,
        r.reserved_at,
        r.checked_in_at,
        r.check_in_method,
        w.name AS workout_name,
        s.start_time,
        s.duration_minutes,
        l.name AS location_name,
        l.location_type
    FROM dbo.reservations r
    JOIN dbo.sessions s ON s.id = r.session_id
    JOIN dbo.workouts w ON w.id = s.workout_id
    JOIN dbo.locations l ON l.id = s.location_id
    WHERE (p_id IS NULL OR r.id = p_id)
      AND (p_user_id IS NULL OR r.user_id = p_user_id)
      AND (p_session_id IS NULL OR r.session_id = p_session_id)
      AND (p_active_only = false OR r.status <> 2)
      AND (p_from IS NULL OR s.start_time >= p_from)
      AND (p_to IS NULL OR s.start_time < p_to)
    ORDER BY s.start_time;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_reservations_by_user(p_user_id integer, p_from timestamp, p_to timestamp)
RETURNS TABLE
(
    id integer, session_id integer, user_id integer, status integer, seat_row integer, seat_column integer,
    reserved_at timestamp, checked_in_at timestamp, check_in_method integer, workout_name varchar,
    start_time timestamp, duration_minutes integer, location_name varchar, location_type integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_reservation_rows(NULL, p_user_id, NULL, false, p_from, p_to);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_active_reservation(p_session_id integer, p_user_id integer)
RETURNS TABLE
(
    id integer, session_id integer, user_id integer, status integer, seat_row integer, seat_column integer,
    reserved_at timestamp, checked_in_at timestamp, check_in_method integer, workout_name varchar,
    start_time timestamp, duration_minutes integer, location_name varchar, location_type integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_reservation_rows(NULL, p_user_id, p_session_id, true, NULL, NULL) LIMIT 1;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_reservation_by_id(p_id integer)
RETURNS TABLE
(
    id integer, session_id integer, user_id integer, status integer, seat_row integer, seat_column integer,
    reserved_at timestamp, checked_in_at timestamp, check_in_method integer, workout_name varchar,
    start_time timestamp, duration_minutes integer, location_name varchar, location_type integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_reservation_rows(p_id, NULL, NULL, false, NULL, NULL);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_save_reservation
(
    p_id integer,
    p_session_id integer,
    p_user_id integer,
    p_status integer,
    p_seat_row integer,
    p_seat_column integer,
    p_reserved_at timestamp,
    p_checked_in_at timestamp,
    p_check_in_method integer
)
RETURNS TABLE
(
    id integer, session_id integer, user_id integer, status integer, seat_row integer, seat_column integer,
    reserved_at timestamp, checked_in_at timestamp, check_in_method integer, workout_name varchar,
    start_time timestamp, duration_minutes integer, location_name varchar, location_type integer
)
LANGUAGE plpgsql
AS $$
DECLARE
    saved_id integer;
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        INSERT INTO dbo.reservations(session_id, user_id, status, seat_row, seat_column, reserved_at, checked_in_at, check_in_method)
        VALUES (p_session_id, p_user_id, p_status, p_seat_row, p_seat_column, p_reserved_at, p_checked_in_at, p_check_in_method)
        RETURNING reservations.id INTO saved_id;
    ELSE
        UPDATE dbo.reservations r
        SET session_id = p_session_id,
            user_id = p_user_id,
            status = p_status,
            seat_row = p_seat_row,
            seat_column = p_seat_column,
            reserved_at = p_reserved_at,
            checked_in_at = p_checked_in_at,
            check_in_method = p_check_in_method
        WHERE r.id = p_id
        RETURNING r.id INTO saved_id;
    END IF;

    RETURN QUERY SELECT * FROM dbo.usp_get_reservation_by_id(saved_id);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_count_user_week_reservations(p_user_id integer, p_week_start timestamp, p_week_end timestamp)
RETURNS integer
LANGUAGE plpgsql
AS $$
DECLARE
    week_count integer;
BEGIN
    SELECT count(*) INTO week_count
    FROM dbo.reservations r
    JOIN dbo.sessions s ON s.id = r.session_id
    WHERE r.user_id = p_user_id
      AND r.status <> 2
      AND s.start_time >= p_week_start
      AND s.start_time < p_week_end;

    RETURN week_count;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_is_seat_taken(p_session_id integer, p_seat_row integer, p_seat_column integer)
RETURNS boolean
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN EXISTS
    (
        SELECT 1
        FROM dbo.reservations r
        WHERE r.session_id = p_session_id
          AND r.status <> 2
          AND r.seat_row = p_seat_row
          AND r.seat_column = p_seat_column
    );
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_reserved_seats(p_session_id integer)
RETURNS TABLE(seat_row integer, seat_column integer)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT r.seat_row, r.seat_column
    FROM dbo.reservations r
    WHERE r.session_id = p_session_id
      AND r.status <> 2
      AND r.seat_row IS NOT NULL
      AND r.seat_column IS NOT NULL;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_session_participants(p_session_id integer)
RETURNS TABLE(user_id integer, user_name varchar, seat_row integer, seat_column integer, is_checked_in boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT u.id AS user_id, u.user_name, r.seat_row, r.seat_column, (r.status = 4) AS is_checked_in
    FROM dbo.reservations r
    JOIN dbo.users u ON u.id = r.user_id
    WHERE r.session_id = p_session_id
      AND r.status <> 2
    ORDER BY u.user_name;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_waitlist(p_session_id integer)
RETURNS TABLE(id integer, session_id integer, user_id integer, queue_position integer, created_at timestamp)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT we.id, we.session_id, we.user_id,
           (row_number() OVER (ORDER BY we.created_at, we.id))::integer AS queue_position,
           we.created_at
    FROM dbo.waitlist_entries we
    WHERE we.session_id = p_session_id
    ORDER BY we.created_at, we.id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_waitlist_entry(p_session_id integer, p_user_id integer)
RETURNS TABLE(id integer, session_id integer, user_id integer, queue_position integer, created_at timestamp)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT we.id, we.session_id, we.user_id, we.queue_position, we.created_at
    FROM dbo.usp_get_waitlist(p_session_id) we
    WHERE we.user_id = p_user_id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_add_waitlist(p_session_id integer, p_user_id integer)
RETURNS TABLE(id integer, session_id integer, user_id integer, queue_position integer, created_at timestamp)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO dbo.waitlist_entries(session_id, user_id)
    VALUES (p_session_id, p_user_id)
    ON CONFLICT (session_id, user_id) DO NOTHING;

    RETURN QUERY SELECT * FROM dbo.usp_get_waitlist_entry(p_session_id, p_user_id);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_remove_waitlist(p_session_id integer, p_user_id integer)
RETURNS boolean
LANGUAGE plpgsql
AS $$
DECLARE
    affected_rows integer;
BEGIN
    DELETE FROM dbo.waitlist_entries we
    WHERE we.session_id = p_session_id AND we.user_id = p_user_id;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows > 0;
END;
$$;
