-- Disse fire linjer vil genstarte opbygningen af id numre korrekt. HUSK at bruge dem når du har slettet en række i en tabel.
-- bare skift alle table_name ud med dem 
CREATE TEMP TABLE temp_table_name AS SELECT *, ROW_NUMBER() OVER (ORDER BY id) as new_id FROM temp_table_name;
UPDATE table_name SET id = temp_table_name.new_id FROM temp_table_name WHERE table_name.id = temp_table_name.id;
SELECT setval('table_name_id_seq', (SELECT MAX(id) FROM table_name));
DROP TABLE temp_table_name;

/*CREATE TEMP TABLE temp_evs AS SELECT *, ROW_NUMBER() OVER (ORDER BY id) as new_id FROM ev_coopers;
UPDATE ev_coopers SET id = temp_evs.new_id FROM temp_evs WHERE ev_coopers.id = temp_evs.id;
SELECT setval('ev_coopers_id_seq', (SELECT MAX(id) FROM ev_coopers));
DROP TABLE temp_table_name;*/

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
    model_name TEXT,
    generation INT,
    model_type TEXT,
    color      TEXT,
    price      INT,
    km_driven  INT,
    max_range  INT,
    weight     INT,
    fuel_type  TEXT,
    geartype   TEXT,
    yearly_tax INT,
    images_id  INT[]
);

CREATE TABLE images
(
    id        SERIAL PRIMARY KEY,
    image_url TEXT
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

SELECT * FROM ev_coopers;

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
    AND t.typtype = 'c'  -- composite types
   OR t.typtype = 'e'  -- enum types
   OR t.typtype = 'r'; -- range types

INSERT INTO users (a_user)
VALUES (
           ROW('John Doe', 'password123', 1234567890, 'john.doe@example.com', 'New York', '123 Main St')::account
       );

INSERT INTO users (a_user)
VALUES (
           ROW('Jane Doe', 'password456', 223344556, 'Dope.doe@example.com', 'New York', '125 Main St')::account
       );

INSERT INTO users (a_user)
VALUES (
           ROW('Jonny Doe', 'password789', 11221122, 'johnny.doe@example.com', 'New York', '124 Main St')::account
       );

INSERT INTO cars (a_car, account_id)
VALUES (
           ROW(
               ROW(  -- Electric Car
                   ROW('Mini Cooper SE', 2022, 'Hatchback', 'Red', 30000, 5000, 250, 1500, 'Electric', 'Automatic', 200, ARRAY[1, 2])::mini_cooper,
                   32,  -- Charge Capacity
                   6.5  -- Km per kWh
                   )::ev_mini_cooper,
               NULL,  -- Fossile Car
               NULL   -- Hybrid Car
               )::car,
           1  -- Assuming this user ID exists in the `users` table
       );

INSERT INTO cars (a_car, account_id)
VALUES (
           ROW(
               NULL,  -- Electric Car
               ROW(  -- Fossile Car
                   ROW('Mini Cooper S', 2021, 'Convertible', 'Blue', 25000, 10000, 0, 1400, 'Gasoline', 'Manual', 150, ARRAY[1])::mini_cooper,
                   55,  -- Tank Capacity
                   15.5,  -- Km per Liter
                   6  -- Gears
                   )::fossil_mini_cooper,
               NULL  -- Hybrid Car
               )::car,
           1  -- Assuming this user ID exists in the `users` table
       );

INSERT INTO cars (a_car, account_id)
VALUES (
           ROW(
               NULL,  -- Electric Car
               NULL,  -- Fossile Car
               ROW(  -- Hybrid Car
                   ROW('Mini Cooper Countryman', 2023, 'SUV', 'Green', 35000, 3000, 200, 1600, 'Hybrid', 'Automatic', 250, ARRAY[2])::mini_cooper,
                   'Gasoline',  -- Fuel Type 1
                   'Electric',  -- Fuel Type 2
                   45,  -- Tank Capacity
                   45,  -- Charge Capacity
                   18.0,  -- Km per Liter
                   5.5,  -- Km per kWh
                   7  -- Gears
                   )::hybrid_mini_cooper
               )::car,
           1  -- Assuming this user ID exists in the `users` table
       );