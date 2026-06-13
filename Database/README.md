# Database

PostgreSQL schema (`dbo`) and stored functions backing the ZeemanSport API. Each `.cs` repository
calls the `dbo.usp_*` functions defined here.

## Apply order

Apply the scripts in this order. The order is required because of foreign keys between tables and
because some functions reference tables created in a later script (PL/pgSQL resolves those
references when the function is *called*, so the whole schema must be applied before the API runs):

1. `users.sql` — creates the `dbo` schema and `users`
2. `workout.sql`
3. `location.sql`
4. `instructor.sql`
5. `session.sql` — FKs to `workouts`, `instructors`, `locations`
6. `subscription.sql` — FK to `users`
7. `reservation.sql` — FKs to `sessions`, `users`; also defines `usp_get_reserved_seats`,
   `usp_count_user_week_reservations`, etc. that `session.sql` functions rely on at call time

Example:

```bash
for f in users workout location instructor session subscription reservation; do
  psql "$CONNECTION_STRING" -f "$f.sql"
done
```

Set the connection string in `ZeemanSport.API/appsettings.Development.json` under
`ConnectionStrings:ZeemanSportDatabase`.

## Convention

Every domain follows the same shape:

- A table with an `IDENTITY` primary key.
- `usp_get_*` read functions returning a `TABLE(...)` row set.
- A single `usp_save_*` upsert (insert when `p_id` is null/0, otherwise update) that returns the
  saved row via the matching `usp_get_*_by_id`.
- A `usp_delete_*` returning a boolean.

Enums are stored as their integer values (see the `Enums.cs` files in `ZeemanSport.Core`).
