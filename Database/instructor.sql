CREATE TABLE IF NOT EXISTS dbo.instructors
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(100) NOT NULL,
    photo_url text,
    is_active boolean NOT NULL DEFAULT true
);

CREATE OR REPLACE FUNCTION dbo.usp_get_instructors()
RETURNS TABLE(id integer, name varchar, photo_url text, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT i.id, i.name, i.photo_url, i.is_active
    FROM dbo.instructors i
    ORDER BY i.name;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_instructor_by_id(p_id integer)
RETURNS TABLE(id integer, name varchar, photo_url text, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT i.id, i.name, i.photo_url, i.is_active
    FROM dbo.instructors i
    WHERE i.id = p_id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_save_instructor(p_id integer, p_name varchar, p_photo_url text, p_is_active boolean)
RETURNS TABLE(id integer, name varchar, photo_url text, is_active boolean)
LANGUAGE plpgsql
AS $$
DECLARE
    saved_id integer;
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        INSERT INTO dbo.instructors(name, photo_url, is_active)
        VALUES (p_name, p_photo_url, p_is_active)
        RETURNING instructors.id INTO saved_id;
    ELSE
        UPDATE dbo.instructors i
        SET name = p_name,
            photo_url = p_photo_url,
            is_active = p_is_active
        WHERE i.id = p_id
        RETURNING i.id INTO saved_id;
    END IF;

    RETURN QUERY
    SELECT * FROM dbo.usp_get_instructor_by_id(saved_id);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_delete_instructor(p_id integer)
RETURNS boolean
LANGUAGE plpgsql
AS $$
DECLARE
    affected_rows integer;
BEGIN
    DELETE FROM dbo.instructors i WHERE i.id = p_id;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows > 0;
END;
$$;
