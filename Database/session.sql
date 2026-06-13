CREATE TABLE IF NOT EXISTS dbo.sessions
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    workout_id integer NOT NULL REFERENCES dbo.workouts(id) ON DELETE CASCADE,
    instructor_id integer NULL REFERENCES dbo.instructors(id) ON DELETE SET NULL,
    location_id integer NOT NULL REFERENCES dbo.locations(id),
    start_time timestamp NOT NULL,
    duration_minutes integer NOT NULL,
    status integer NOT NULL DEFAULT 1
);

CREATE INDEX IF NOT EXISTS ix_sessions_start_time ON dbo.sessions(start_time);
CREATE INDEX IF NOT EXISTS ix_sessions_instructor_id ON dbo.sessions(instructor_id);

CREATE OR REPLACE FUNCTION dbo.usp_get_session_rows(p_id integer, p_instructor_id integer, p_from timestamp, p_to timestamp)
RETURNS TABLE
(
    id integer,
    workout_id integer,
    workout_name varchar,
    instructor_id integer,
    instructor_name varchar,
    location_id integer,
    location_name varchar,
    location_type integer,
    start_time timestamp,
    duration_minutes integer,
    status integer,
    capacity integer,
    width_in_seats integer,
    height_in_seats integer,
    reserved_count integer,
    waitlist_count integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        s.id,
        s.workout_id,
        w.name AS workout_name,
        s.instructor_id,
        i.name AS instructor_name,
        s.location_id,
        l.name AS location_name,
        l.location_type,
        s.start_time,
        s.duration_minutes,
        s.status,
        l.capacity,
        l.width_in_seats,
        l.height_in_seats,
        COALESCE((SELECT count(*) FROM dbo.reservations r WHERE r.session_id = s.id AND r.status <> 2), 0)::integer AS reserved_count,
        COALESCE((SELECT count(*) FROM dbo.waitlist_entries we WHERE we.session_id = s.id), 0)::integer AS waitlist_count
    FROM dbo.sessions s
    JOIN dbo.workouts w ON w.id = s.workout_id
    JOIN dbo.locations l ON l.id = s.location_id
    LEFT JOIN dbo.instructors i ON i.id = s.instructor_id
    WHERE (p_id IS NULL OR s.id = p_id)
      AND (p_instructor_id IS NULL OR s.instructor_id = p_instructor_id)
      AND (p_from IS NULL OR s.start_time >= p_from)
      AND (p_to IS NULL OR s.start_time < p_to)
    ORDER BY s.start_time;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_schedule(p_from timestamp, p_to timestamp)
RETURNS TABLE
(
    id integer, workout_id integer, workout_name varchar, instructor_id integer, instructor_name varchar,
    location_id integer, location_name varchar, location_type integer, start_time timestamp, duration_minutes integer,
    status integer, capacity integer, width_in_seats integer, height_in_seats integer, reserved_count integer, waitlist_count integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_session_rows(NULL, NULL, p_from, p_to);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_session_by_id(p_id integer)
RETURNS TABLE
(
    id integer, workout_id integer, workout_name varchar, instructor_id integer, instructor_name varchar,
    location_id integer, location_name varchar, location_type integer, start_time timestamp, duration_minutes integer,
    status integer, capacity integer, width_in_seats integer, height_in_seats integer, reserved_count integer, waitlist_count integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_session_rows(p_id, NULL, NULL, NULL);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_instructor_sessions(p_instructor_id integer, p_from timestamp, p_to timestamp)
RETURNS TABLE
(
    id integer, workout_id integer, workout_name varchar, instructor_id integer, instructor_name varchar,
    location_id integer, location_name varchar, location_type integer, start_time timestamp, duration_minutes integer,
    status integer, capacity integer, width_in_seats integer, height_in_seats integer, reserved_count integer, waitlist_count integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_session_rows(NULL, p_instructor_id, p_from, p_to);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_save_session
(
    p_id integer,
    p_workout_id integer,
    p_instructor_id integer,
    p_location_id integer,
    p_start_time timestamp,
    p_duration_minutes integer,
    p_status integer
)
RETURNS TABLE
(
    id integer, workout_id integer, workout_name varchar, instructor_id integer, instructor_name varchar,
    location_id integer, location_name varchar, location_type integer, start_time timestamp, duration_minutes integer,
    status integer, capacity integer, width_in_seats integer, height_in_seats integer, reserved_count integer, waitlist_count integer
)
LANGUAGE plpgsql
AS $$
DECLARE
    saved_id integer;
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        INSERT INTO dbo.sessions(workout_id, instructor_id, location_id, start_time, duration_minutes, status)
        VALUES (p_workout_id, p_instructor_id, p_location_id, p_start_time, p_duration_minutes, p_status)
        RETURNING sessions.id INTO saved_id;
    ELSE
        UPDATE dbo.sessions s
        SET workout_id = p_workout_id,
            instructor_id = p_instructor_id,
            location_id = p_location_id,
            start_time = p_start_time,
            duration_minutes = p_duration_minutes,
            status = p_status
        WHERE s.id = p_id
        RETURNING s.id INTO saved_id;
    END IF;

    RETURN QUERY SELECT * FROM dbo.usp_get_session_by_id(saved_id);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_delete_session(p_id integer)
RETURNS boolean
LANGUAGE plpgsql
AS $$
DECLARE
    affected_rows integer;
BEGIN
    DELETE FROM dbo.sessions s WHERE s.id = p_id;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows > 0;
END;
$$;
