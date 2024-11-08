CREATE TYPE odd_even AS
    (
    even INT[],
    odd  INT[]
    );

CREATE TABLE classes
(
    id         SERIAL,
    odds_evens odd_even
);

INSERT INTO classes(odds_evens)
VALUES (ROW (ARRAY [2, 4, 6], ARRAY [1, 3, 5]));

SELECT (odds_evens).even
FROM classes;

SELECT array_append(odds_evens.odd, 1)
FROM classes
WHERE id = 1;

/* Database setup with classes. */
CREATE TABLE persons
(
    id     SERIAL PRIMARY KEY,
    person person[]
);

CREATE TYPE person as
    (
    id              INT,
    name            TEXT,
    mobile          INT,
    email           TEXT,
    listed_vehicles car[]
    );

CREATE TYPE car AS
    (
    user_id     INT,
    price       INT,
    doors       INT,
    brand       TEXT,
    model       TEXT,
    km_driven   INT,
    capacity    INT,
/*    picture_ids INT[],*/
    pictures picture[]
    );

CREATE TYPE picture AS
    (
    id      INT,
    picture TEXT
    );

CREATE TABLE pictures
(
    id      SERIAL PRIMARY KEY,
    picture TEXT
);