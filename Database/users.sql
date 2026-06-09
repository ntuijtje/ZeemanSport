CREATE SCHEMA IF NOT EXISTS dbo;

CREATE TABLE IF NOT EXISTS dbo.users
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_name varchar(100) NOT NULL UNIQUE,
    first_name varchar(100),
    last_name varchar(100),
    user_role integer NOT NULL,
    password_hash text NOT NULL
);

CREATE OR REPLACE FUNCTION dbo.usp_get_user_by_id(p_id integer)
RETURNS TABLE
(
    id integer,
    user_name varchar,
    first_name varchar,
    last_name varchar,
    user_role integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        users.id,
        users.user_name,
        users.first_name,
        users.last_name,
        users.user_role
    FROM dbo.users
    WHERE users.id = p_id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_user_by_user_name(p_user_name varchar)
RETURNS TABLE
(
    id integer,
    user_name varchar,
    first_name varchar,
    last_name varchar,
    user_role integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        users.id,
        users.user_name,
        users.first_name,
        users.last_name,
        users.user_role
    FROM dbo.users
    WHERE lower(users.user_name) = lower(p_user_name);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_user_password_hash(p_user_id integer)
RETURNS TABLE
(
    password_hash text
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT users.password_hash
    FROM dbo.users
    WHERE users.id = p_user_id;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_save_user
(
    p_id integer,
    p_user_name varchar,
    p_first_name varchar,
    p_last_name varchar,
    p_user_role integer,
    p_password_hash text
)
RETURNS TABLE
(
    id integer,
    user_name varchar,
    first_name varchar,
    last_name varchar,
    user_role integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        RETURN QUERY
        INSERT INTO dbo.users
        (
            user_name,
            first_name,
            last_name,
            user_role,
            password_hash
        )
        VALUES
        (
            p_user_name,
            p_first_name,
            p_last_name,
            p_user_role,
            p_password_hash
        )
        RETURNING
            users.id,
            users.user_name,
            users.first_name,
            users.last_name,
            users.user_role;

        RETURN;
    END IF;

    RETURN QUERY
    UPDATE dbo.users
    SET
        user_name = p_user_name,
        first_name = p_first_name,
        last_name = p_last_name,
        user_role = p_user_role,
        password_hash = COALESCE(p_password_hash, users.password_hash)
    WHERE users.id = p_id
    RETURNING
        users.id,
        users.user_name,
        users.first_name,
        users.last_name,
        users.user_role;
END;
$$;
