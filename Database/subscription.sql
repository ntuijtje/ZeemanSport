CREATE TABLE IF NOT EXISTS dbo.subscription_plans
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name varchar(100) NOT NULL UNIQUE,
    access_tier integer NOT NULL,
    billing_interval integer NOT NULL,
    price numeric(10, 2) NOT NULL,
    is_active boolean NOT NULL DEFAULT true
);

CREATE TABLE IF NOT EXISTS dbo.subscriptions
(
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id integer NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE,
    plan_id integer NOT NULL REFERENCES dbo.subscription_plans(id),
    start_date date NOT NULL,
    end_date date NOT NULL,
    status integer NOT NULL DEFAULT 1
);

CREATE INDEX IF NOT EXISTS ix_subscriptions_user_id ON dbo.subscriptions(user_id);

CREATE OR REPLACE FUNCTION dbo.usp_get_subscription_plans()
RETURNS TABLE(id integer, name varchar, access_tier integer, billing_interval integer, price numeric, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT p.id, p.name, p.access_tier, p.billing_interval, p.price, p.is_active
    FROM dbo.subscription_plans p
    WHERE p.is_active = true
    ORDER BY p.price;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_subscription_plan_by_id(p_id integer)
RETURNS TABLE(id integer, name varchar, access_tier integer, billing_interval integer, price numeric, is_active boolean)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT p.id, p.name, p.access_tier, p.billing_interval, p.price, p.is_active
    FROM dbo.subscription_plans p
    WHERE p.id = p_id;
END;
$$;

-- Returns subscriptions enriched with their plan details. p_user_id / p_id / p_active_only filter
-- the same base query so the C# repository maps a single consistent column set.
CREATE OR REPLACE FUNCTION dbo.usp_get_subscription_rows(p_id integer, p_user_id integer, p_active_only boolean)
RETURNS TABLE
(
    id integer,
    user_id integer,
    plan_id integer,
    plan_name varchar,
    access_tier integer,
    billing_interval integer,
    price numeric,
    start_date date,
    end_date date,
    status integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        s.id,
        s.user_id,
        s.plan_id,
        p.name AS plan_name,
        p.access_tier,
        p.billing_interval,
        p.price,
        s.start_date,
        s.end_date,
        s.status
    FROM dbo.subscriptions s
    JOIN dbo.subscription_plans p ON p.id = s.plan_id
    WHERE (p_id IS NULL OR s.id = p_id)
      AND (p_user_id IS NULL OR s.user_id = p_user_id)
      AND (
            p_active_only = false
            OR (s.status = 2 AND s.start_date <= current_date AND s.end_date >= current_date)
          )
    ORDER BY s.end_date DESC;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_subscriptions_by_user(p_user_id integer)
RETURNS TABLE
(
    id integer, user_id integer, plan_id integer, plan_name varchar, access_tier integer,
    billing_interval integer, price numeric, start_date date, end_date date, status integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_subscription_rows(NULL, p_user_id, false);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_active_subscription(p_user_id integer)
RETURNS TABLE
(
    id integer, user_id integer, plan_id integer, plan_name varchar, access_tier integer,
    billing_interval integer, price numeric, start_date date, end_date date, status integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_subscription_rows(NULL, p_user_id, true) LIMIT 1;
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_get_subscription_by_id(p_id integer)
RETURNS TABLE
(
    id integer, user_id integer, plan_id integer, plan_name varchar, access_tier integer,
    billing_interval integer, price numeric, start_date date, end_date date, status integer
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY SELECT * FROM dbo.usp_get_subscription_rows(p_id, NULL, false);
END;
$$;

CREATE OR REPLACE FUNCTION dbo.usp_save_subscription
(
    p_id integer,
    p_user_id integer,
    p_plan_id integer,
    p_start_date date,
    p_end_date date,
    p_status integer
)
RETURNS TABLE
(
    id integer, user_id integer, plan_id integer, plan_name varchar, access_tier integer,
    billing_interval integer, price numeric, start_date date, end_date date, status integer
)
LANGUAGE plpgsql
AS $$
DECLARE
    saved_id integer;
BEGIN
    IF p_id IS NULL OR p_id = 0 THEN
        INSERT INTO dbo.subscriptions(user_id, plan_id, start_date, end_date, status)
        VALUES (p_user_id, p_plan_id, p_start_date, p_end_date, p_status)
        RETURNING subscriptions.id INTO saved_id;
    ELSE
        UPDATE dbo.subscriptions s
        SET user_id = p_user_id,
            plan_id = p_plan_id,
            start_date = p_start_date,
            end_date = p_end_date,
            status = p_status
        WHERE s.id = p_id
        RETURNING s.id INTO saved_id;
    END IF;

    RETURN QUERY SELECT * FROM dbo.usp_get_subscription_by_id(saved_id);
END;
$$;

INSERT INTO dbo.subscription_plans(name, access_tier, billing_interval, price, is_active)
VALUES
    ('Twice a week - monthly', 1, 1, 29.00, true),
    ('Twice a week - yearly', 1, 2, 299.00, true),
    ('Unlimited - monthly', 2, 1, 55.00, true),
    ('Unlimited - yearly', 2, 2, 549.00, true)
ON CONFLICT (name) DO UPDATE
SET access_tier = EXCLUDED.access_tier,
    billing_interval = EXCLUDED.billing_interval,
    price = EXCLUDED.price,
    is_active = EXCLUDED.is_active;
