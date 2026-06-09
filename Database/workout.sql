CREATE TABLE IF NOT EXISTS dbo.workouts
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(100) NOT NULL UNIQUE,
    description text,
    is_active boolean NOT NULL DEFAULT true
);

CREATE OR REPLACE FUNCTION dbo.usp_get_workouts()
RETURNS TABLE(id integer, name varchar, description text, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT w.id, w.name, w.description, w.is_active
    FROM dbo.workouts w
    ORDER BY w.name;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_workout_by_id(p_id integer)
RETURNS TABLE(id integer, name varchar, description text, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT w.id, w.name, w.description, w.is_active
    FROM dbo.workouts w
    WHERE w.id = p_id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_save_workout(p_id integer, p_name varchar, p_description text, p_is_active boolean)
RETURNS TABLE(id integer, name varchar, description text, is_active boolean)
LANGUAGE plpgsql
AS $$
DECLARE
    saved_id integer;
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        INSERT INTO dbo.workouts(name, description, is_active)
        VALUES (p_name, p_description, p_is_active)
        RETURNING workouts.id INTO saved_id;
    ELSE
        UPDATE dbo.workouts w
        SET name = p_name,
            description = p_description,
            is_active = p_is_active,
            updated_at = current_timestamp
        WHERE w.id = p_id
        RETURNING w.id INTO saved_id;
    END IF;

    RETURN QUERY
    SELECT * FROM dbo.usp_get_workout_by_id(saved_id);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_delete_workout(p_id integer)
RETURNS boolean
LANGUAGE plpgsql
AS $$
DECLARE
    affected_rows integer;
BEGIN
    DELETE FROM dbo.workouts w WHERE w.id = p_id;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows > 0;
END;
$$;
