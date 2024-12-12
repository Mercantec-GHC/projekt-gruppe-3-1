-- Disse fire linjer vil genstarte opbygningen af id numre korrekt. HUSK at bruge dem når du har slettet en række i en tabel.
-- bare skift alle table_name ud med dem 
CREATE TEMP TABLE temp_users AS
SELECT *, ROW_NUMBER() OVER (ORDER BY id) as new_id
FROM users;
UPDATE users
SET id = temp_users.new_id
FROM temp_users
WHERE users.id = temp_users.id;
SELECT setval('users_id_seq', (SELECT MAX(id) FROM users));
DROP TABLE temp_users;

DELETE FROM users;

SELECT * FROM users;
SELECT id, (a_user).name,(a_user).password, (a_user).mobile, (a_user).email, (a_user).city, (a_user).address FROM users WHERE (a_user).email = 'email' AND (a_user).password = 'you';

/*CREATE TABLE images
(
    id        SERIAL PRIMARY KEY,
    image_url TEXT
);
DROP TABLE images CASCADE;*/

/*CREATE TEMP TABLE temp_evs AS SELECT *, ROW_NUMBER() OVER (ORDER BY id) as new_id FROM ev_coopers;
UPDATE ev_coopers SET id = temp_evs.new_id FROM temp_evs WHERE ev_coopers.id = temp_evs.id;
SELECT setval('ev_coopers_id_seq', (SELECT MAX(id) FROM ev_coopers));
DROP TABLE temp_table_name;*/

/*DROP TABLE fossil_coopers;
DROP TABLE ev_coopers;
DROP TABLE hybrid_coopers;*/

-- CASCADE drops other stuff that is depending on that type/table.
CREATE TYPE account AS
(
    name     TEXT,
    password TEXT,
    mobile   INT,
    email    TEXT,
    city     TEXT,
    address  TEXT
);

CREATE TABLE users
(
    id     SERIAL PRIMARY KEY,
    a_user account
);

-- Basen af vores mini coopers.
CREATE TYPE mini_cooper AS
(
    model_name    TEXT,
    generation    INT,
    model_type    TEXT,
    color         TEXT,
    price         INT,
    km_driven     INT,
    max_range     INT,
    weight        INT,
    fuel_type     TEXT,
    geartype      TEXT,
    yearly_tax    INT,
    base64_images TEXT[]
);



CREATE TYPE ev_mini_cooper AS
(
    base_cooper     mini_cooper,
    charge_capacity INT,
    km_pr_kwh       FLOAT
);

CREATE TYPE fossil_mini_cooper AS
(
    base_cooper   mini_cooper,
    tank_capacity INT,
    km_pr_liter   FLOAT,
    gears         INT
);

CREATE TYPE hybrid_mini_cooper AS
(
    base_cooper     mini_cooper,
    fuel_type1      TEXT,
    fuel_type2      TEXT,
    tank_capacity   INT,
    charge_capacity INT,
    km_pr_liter     FLOAT,
    km_pr_kwh       FLOAT,
    gears           INT
);

-- Meningen er at der kun skal være en enkelt værdi, mens de to andre værdier er NULL/tomme.
CREATE TYPE car AS
(
    electric_car ev_mini_cooper,
    fossile_car  fossil_mini_cooper,
    hybrid_car   hybrid_mini_cooper
);

CREATE TABLE cars
(
    id         SERIAL PRIMARY KEY,
    a_car      car,
    account_id INT,
    CONSTRAINT FK_account_id FOREIGN KEY (account_id) REFERENCES users (id)
);

DROP TYPE fossil_mini_cooper CASCADE;
DROP TYPE ev_mini_cooper CASCADE;
DROP TYPE mini_cooper CASCADE;
DROP TYPE hybrid_mini_cooper CASCADE;
DROP TYPE car CASCADE;
DROP TYPE account CASCADE;
DROP TABLE users CASCADE;
DROP TABLE cars CASCADE;

SELECT *
FROM cars;

-- Viser alle tabeller i en database.
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public';

-- Viser alle bruger defineret typer/klasser i en database.
SELECT n.nspname as schema, t.typname as type
FROM pg_type t
         JOIN pg_catalog.pg_namespace n ON n.oid = t.typnamespace
WHERE n.nspname != 'pg_catalog'
    AND n.nspname != 'information_schema'
    AND t.typtype = 'c' -- composite types
   OR t.typtype = 'e'   -- enum types
   OR t.typtype = 'r'; -- range types

INSERT INTO users (a_user)
VALUES (ROW ('John Doe', 'password123', 1234567890, 'john.doe@example.com', 'New York', '123 Main St')::account);

INSERT INTO users (a_user)
VALUES (ROW ('Jane Doe', 'password456', 223344556, 'Dope.doe@example.com', 'New York', '125 Main St')::account);

INSERT INTO users (a_user)
VALUES (ROW ('Jonny Doe', 'password789', 11221122, 'johnny.doe@example.com', 'New York', '124 Main St')::account);

INSERT INTO cars (a_car, account_id)
VALUES (ROW (ROW (ROW ('Mini Cooper Electric', 2023, 'Crossover', 'Silver', 32000, 2000, 270, 1400, 'Electric', 'Automatic', 180, ARRAY ['base64string1', 'base64string2'])::mini_cooper, 40, 7.2)::ev_mini_cooper, NULL, NULL)::car,
        1);

INSERT INTO cars (a_car, account_id)
VALUES (ROW (
            NULL, -- Electric Car
            NULL, -- Fossile Car
            ROW ( -- Hybrid Car
                ROW ('Mini Cooper Hybrid', 2023, 'Coupe', 'Yellow', 36000, 5000, 220, 1500, 'Hybrid', 'Automatic', 240, ARRAY ['base64string5'])::mini_cooper,
                'Petrol', -- Fuel Type 1
                'Electric', -- Fuel Type 2
                50, -- Tank Capacity
                35, -- Charge Capacity
                19.0, -- Km per Liter
                5.2, -- Km per kWh
                6 -- Gears
                )::hybrid_mini_cooper
            )::car,
        1 -- Assuming this user ID exists in the `users` table
       );

INSERT INTO cars (a_car, account_id)
VALUES (ROW (
            NULL, -- Electric Car
            ROW ( -- Fossile Car
                ROW ('Mini Cooper Classic', 2020, 'Sedan', 'Black', 22000, 15000, 0, 1300, 'Diesel', 'Manual', 180, ARRAY ['base64string3', 'base64string4'])::mini_cooper,
                60, -- Tank Capacity
                16.0, -- Km per Liter
                5 -- Gears
                )::fossil_mini_cooper,
            NULL -- Hybrid Car
            )::car,
        1 -- Assuming this user ID exists in the `users` table
       );

DELETE
FROM cars;

SELECT a_car, account_id
FROM cars;

SELECT (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car, account_id
FROM cars;

SELECT a_car, account_id
FROM cars
WHERE (a_car).electric_car IS NOT NULL;

SELECT CASE
           WHEN (a_car).electric_car IS NOT NULL THEN (a_car).electric_car
           WHEN (a_car).fossile_car IS NOT NULL THEN (a_car).fossile_car
           WHEN (a_car).hybrid_car IS NOT NULL THEN (a_car).hybrid_car END AS tester
FROM cars;

SELECT CASE
           WHEN (a_car).electric_car IS NOT NULL THEN (a_car).electric_car
           WHEN (a_car).fossile_car IS NOT NULL THEN (a_car).fossile_car
           WHEN (a_car).hybrid_car IS NOT NULL THEN (a_car).hybrid_car
           END AS non_null_car,
       account_id
FROM cars
WHERE (a_car).electric_car IS NOT NULL
   OR (a_car).fossile_car IS NOT NULL
   OR (a_car).hybrid_car IS NOT NULL;

SELECT (a_car).fossile_car
FROM cars
WHERE (a_car).electric_car IS NOT NULL;

-- carEvBase carEv
SELECT (a_car).electric_car, (a_car).fossile_car, (a_car).hybrid_car
FROM cars;

SELECT (a_car).electric_car.base_cooper.model_name,
       (a_car).electric_car.base_cooper.generation,
       (a_car).electric_car.base_cooper.color,
       (a_car).fossile_car.base_cooper.model_name,
       (a_car).fossile_car.base_cooper.generation,
       (a_car).fossile_car.base_cooper.color,
       (a_car).hybrid_car.base_cooper.model_name,
       (a_car).hybrid_car.base_cooper.generation,
       (a_car).hybrid_car.base_cooper.color,
       account_id
FROM cars;

SELECT *
FROM cars;

SELECT (a_car).electric_car.km_pr_kwh FROM cars;

SELECT images
FROM cars, unnest((a_car).electric_car.base_cooper.base64_images) AS images WHERE id = 2;

SELECT (a_car).electric_car.base_cooper.model_name, (a_car).electric_car.base_cooper.generation, (a_car).electric_car.base_cooper.color, (a_car).electric_car.base_cooper.price, (a_car).electric_car.base_cooper.km_driven, (a_car).electric_car.base_cooper.max_range, (a_car).electric_car.base_cooper.weight, (a_car).electric_car.base_cooper.fuel_type, (a_car).electric_car.base_cooper.geartype, (a_car).electric_car.base_cooper.yearly_tax, (a_car).electric_car.charge_capacity, (a_car).electric_car.km_pr_kwh FROM cars WHERE id = 2;

SELECT (a_car).electric_car.base_cooper.model_name, (a_car).electric_car.base_cooper.generation, (a_car).electric_car.base_cooper.color, (a_car).electric_car.base_cooper.price, (a_car).electric_car.base_cooper.km_driven, (a_car).electric_car.base_cooper.max_range, (a_car).electric_car.base_cooper.weight, (a_car).electric_car.base_cooper.fuel_type, (a_car).electric_car.base_cooper.geartype, (a_car).electric_car.base_cooper.yearly_tax, (a_car).electric_car.charge_capacity, (a_car).electric_car.km_pr_kwh FROM cars WHERE id = 2;

SELECT * FROM cars WHERE id = 99;

SELECT (a_user).email FROM users WHERE (a_user).email = 'jogn.doe@example.com';

-- Inserting a user example
INSERT INTO users (a_user)
VALUES (ROW ('Alice Smith', 'pass123', 9876543210, 'alice.smith@example.com', 'Los Angeles', '456 Elm St')::account);

-- Inserting another user
INSERT INTO users (a_user) VALUES (ROW ('Bob Brown', 'mypassword', 87654321, 'bob.brown@example.com', 'San Francisco', '789 Pine St')::account);

SELECT * FROM users;

UPDATE users SET a_user.name = 'Mark6' WHERE id = 10;

SELECT id, (a_user).name FROM users ORDER BY id;

UPDATE cars SET a_car.fossile_car.base_cooper.fuel_type = 'Benzin' WHERE id =9;

SELECT * FROM cars;