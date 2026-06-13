CREATE TABLE IF NOT EXISTS dbo.locations
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(100) NOT NULL UNIQUE,
    location_type integer NOT NULL,
    capacity integer NOT NULL,
    width_in_seats integer NOT NULL,
    height_in_seats integer NOT NULL,
    is_active boolean NOT NULL DEFAULT true
);

CREATE OR REPLACE FUNCTION dbo.usp_get_locations()
RETURNS TABLE(id integer, name varchar, location_type integer, capacity integer, width_in_seats integer, height_in_seats integer, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT l.id, l.name, l.location_type, l.capacity, l.width_in_seats, l.height_in_seats, l.is_active
    FROM dbo.locations l
    ORDER BY l.id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_location_by_id(p_id integer)
RETURNS TABLE(id integer, name varchar, location_type integer, capacity integer, width_in_seats integer, height_in_seats integer, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT l.id, l.name, l.location_type, l.capacity, l.width_in_seats, l.height_in_seats, l.is_active
    FROM dbo.locations l
    WHERE l.id = p_id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_save_location
(
    p_id integer,
    p_name varchar,
    p_location_type integer,
    p_capacity integer,
    p_width_in_seats integer,
    p_height_in_seats integer,
    p_is_active boolean
)
RETURNS TABLE(id integer, name varchar, location_type integer, capacity integer, width_in_seats integer, height_in_seats integer, is_active boolean)
LANGUAGE plpgsql
AS $$
DECLARE
    saved_id integer;
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        INSERT INTO dbo.locations(name, location_type, capacity, width_in_seats, height_in_seats, is_active)
        VALUES (p_name, p_location_type, p_capacity, p_width_in_seats, p_height_in_seats, p_is_active)
        RETURNING locations.id INTO saved_id;
    ELSE
        UPDATE dbo.locations l
        SET name = p_name,
            location_type = p_location_type,
            capacity = p_capacity,
            width_in_seats = p_width_in_seats,
			height_in_seats = p_height_in_seats,
            is_active = p_is_active
        WHERE l.id = p_id
        RETURNING l.id INTO saved_id;
    END IF;

    RETURN QUERY
    SELECT * FROM dbo.usp_get_location_by_id(saved_id);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_delete_location(p_id integer)
RETURNS boolean
LANGUAGE plpgsql
AS $$
DECLARE
    affected_rows integer;
BEGIN
    DELETE FROM dbo.locations l WHERE l.id = p_id;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows > 0;
END;
$$;

INSERT INTO dbo.locations(name, location_type, capacity, width_in_seats, height_in_seats, is_active)
VALUES
    ('Room 1', 1, 42, 0,0, true),
    ('Room 2', 1, 32, 0,0, true),
    ('Room 3', 1, 24, 0,0, true),
    ('Outdoor Area', 2, 20, 0,0, true),
    ('Spinning Room', 3, 24, 6,4, true)
ON CONFLICT (name) DO UPDATE
SET
    location_type = EXCLUDED.location_type,
    capacity = EXCLUDED.capacity,
    width_in_seats = EXCLUDED.width_in_seats,
    height_in_seats = EXCLUDED.height_in_seats,
    is_active = EXCLUDED.is_active;
