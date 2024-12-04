-- Disse fire linjer vil genstarte opbygningen af id numre korrekt. HUSK at bruge dem når du har slettet en række i en tabel.
-- bare skift alle table_name ud med dem 
CREATE TEMP TABLE temp_cars AS
SELECT *, ROW_NUMBER() OVER (ORDER BY id) as new_id
FROM cars;
UPDATE cars
SET id = temp_cars.new_id
FROM temp_cars
WHERE cars.id = temp_cars.id;
SELECT setval('cars_id_seq', (SELECT MAX(id) FROM cars));
DROP TABLE temp_cars;

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
VALUES (ROW (ROW (ROW ('Mini Cooper Electric', 2023, 'Crossover', 'Silver', 32000, 2000, 270, 1400, 'Electric', 'Automatic', 180, ARRAY ['base64string1', 'base64string2'])::mini_cooper, 40, 7.2)::ev_mini_cooper, NULL, NULL)::car,1);

DELETE
FROM cars;

