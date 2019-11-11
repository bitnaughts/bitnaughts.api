---------------------------
-- MISCELLANEOUS QUERIES --
---------------------------

-- get number of players currently online
SELECT COUNT(*)
FROM dbo.Players
WHERE py_current_sesson > -1;

-- get names of ships who have started a fight with another ship
SELECT sh_name
FROM dbo.Ships INNER JOIN ( 
    SELECT ch_ship_1
    FROM dbo.CombatHistory
) ch ON ch_ship_1 = sh_ship_id;

-- get asteroids and their average size that have been mined before but are not completely destroyed
SELECT a_asteroid_id, AVG(a_size)
FROM dbo.Asteroids
INNER JOIN (
    SELECT m_asteroid_id
    FROM dbo.Mines
) m ON m_asteroid_id = a_asteroid_id

--------------------------
-- API ENDPOINT QUERIES --
--------------------------

-- api/login?player=1

UPDATE dbo.Players SET py_current_session = 0 WHERE py_player_id = 1;
INSERT INTO dbo.SessionHistory
VALUES
    (0, 1, '2019-11-11 07:25:05.752', NULL);

-- api/logout?player=1

UPDATE dbo.Players SET py_current_session = -1 WHERE py_player_id = 1;
UPDATE dbo.SessionHistory SET sh_logout = '2019-11-11 07:26:09.714' WHERE sh_session_id = 1;

-- api/fight?ship_1=0&ship_2=1

INSERT INTO dbo.CombatHistory
VALUES
    (0, 0, 1, '2019-11-11 06:10:56.520')

-- api/visit?planet=12&ship=0

INSERT INTO dbo.Visits
VALUES
    (0, 12, '2019-11-11 05:31:57.491');

-- api/mine?asteroid=11&ship=0&amount=20

-- Mining only part of an asteroid
UPDATE dbo.Asteroids SET a_size = 6.59 WHERE a_asteroid_id = 11;
UPDATE dbo.Ships SET sp_data = 1.00 WHERE sp_ship_id = 0;
INSERT INTO dbo.Mines
VALUES
    (0, 11, 1.00, '2019-11-11 05:28:20.408');

-- Mining an asteroid completely
DELETE FROM dbo.Asteroids WHERE a_asteroid_id = 12;
UPDATE dbo.Ships SET sp_data = 0.20 WHERE sp_ship_id = 0;
INSERT INTO dbo.Mines
VALUES
    (0, 12, .20, '2019-11-11 05:28:24.243');

-- api/set?flag=add&table=dbo.ships

INSERT INTO dbo.Ships
VALUES
    (0, 1, 1, 'ShipX', '', 4.24, 2.42);

-- api/set?flag=reset

-- Deletion Commands executed by set&flag=reset
DELETE FROM dbo.Galaxies;
DELETE FROM dbo.Systems;
DELETE FROM dbo.SystemConnections;
DELETE FROM dbo.Planets;
DELETE FROM dbo.Asteroids;
-- Insertion Commands excuted by set&flag=reset
INSERT INTO dbo.Galaxies
VALUES
    (0, 10);
INSERT INTO dbo.Systems
VALUES
    (0, 0, 1613858733, 0.47, 7.40),
    (1, 0, 2038138748, -2.40, -9.94),
    (2, 0, 1595134347, 0.57, -2.57),
    (3, 0, 1375440120, -9.38, 6.32),
    (4, 0, 2023539885, -18.38, -1.43),
    (5, 0, 1815309053, -1.08, -1.44),
    (6, 0, 783319722, 2.08, -11.87),
    (7, 0, 371979998, -0.86, -0.64),
    (8, 0, 1312278758, 2.63, -2.40);
INSERT INTO dbo.SystemConnections
VALUES
    (0, 7, 1),
    (0, 5, 1),
    (1, 6, 1),
    (1, 2, 1),
    (1, 5, 1),
    (1, 8, 1),
    (2, 5, 1),
    (2, 8, 1),
    (2, 7, 1),
    (2, 1, 1),
    (3, 0, 1),
    (3, 7, 1),
    (4, 3, 1),
    (4, 5, 1),
    (4, 7, 1),
    (4, 1, 1),
    (5, 7, 1),
    (5, 2, 1),
    (5, 8, 1),
    (5, 1, 1),
    (6, 1, 1),
    (6, 2, 1),
    (7, 5, 1),
    (7, 2, 1),
    (8, 2, 1);
INSERT INTO dbo.SystemConnections
VALUES
    (8, 5, 1),
    (8, 7, 1),
    (8, 1, 1);
INSERT INTO dbo.Planets
VALUES
    (0, 0, 1366950329, -4.58, -7.95, 38.66, 4.72, 'metallic', 1, 1, 0.00, 'command mining data'),
    (1, 0, 668616053, 1.49, -6.26, 15.00, 2.73, 'carbonaceous', 1, 1, 0.00, 'command metallurgic service'),
    (2, 0, 1929695548, 6.16, -5.70, 18.83, 3.30, 'silicaceous', 1, 1, 0.00, 'mixed energy extraction'),
    (3, 0, 2104143775, -7.38, 0.53, 75.81, 2.34, 'silicaceous', 1, 1, 0.00, 'market philosphical service'),
    (4, 0, 37241412, -5.12, 1.90, 55.40, 4.37, 'carbonaceous', 1, 1, 0.00, 'command philosphical data');
INSERT INTO dbo.Planets
VALUES
    (25, 5, 1737142424, -6.12, 3.14, 90.38, 2.91, 'carbonaceous', 1, 1, 0.00, 'command chemical extraction'),
    (26, 6, 904884396, -5.53, 5.86, 40.83, 5.41, 'silicaceous', 1, 1, 0.00, 'mixed mining manufacturing'),
    (27, 6, 1517933281, -1.71, -9.71, 57.99, 3.44, 'carbonaceous', 1, 1, 0.00, 'mixed philosphical extraction'),
    (28, 6, 2043467221, 6.65, -5.24, 26.35, 4.82, 'silicaceous', 1, 1, 0.00, 'traditional metallurgic data');
INSERT INTO dbo.Asteroids
VALUES
    (6, 0, 1774790354, 5.17, 0.01, 0.00, 0.00, 'NULL', 1, 0),
    (7, 0, 1781294417, 2.72, 5.29, 0.00, 0.00, 'NULL', 1, 0),
    (8, 0, 1623045480, 4.37, -6.45, 0.00, 0.00, 'NULL', 1, 0),
    (9, 0, 616211639, 8.17, 4.20, 0.00, 0.00, 'NULL', 1, 0),
    (10, 0, 46235655, 1.67, -6.37, 0.00, 0.00, 'NULL', 1, 0),
    (11, 0, 740640215, 4.41, -7.55, 0.00, 0.00, 'NULL', 1, 0),
    (12, 0, 682378605, 1.12, -9.13, 0.00, 0.00, 'NULL', 1, 0),
    (13, 0, 1335452811, -5.43, 0.77, 0.00, 0.00, 'NULL', 1, 0),
    (14, 0, 1087810822, -4.98, 3.24, 0.00, 0.00, 'NULL', 1, 0);
INSERT INTO dbo.Asteroids
VALUES
    (39, 1, 1810164066, -3.76, -7.29, 0.00, 0.00, 'NULL', 1, 0),
    (40, 1, 1494847465, -6.46, -0.23, 0.00, 0.00, 'NULL', 1, 0),
    (41, 1, 856590285, 6.50, 3.74, 0.00, 0.00, 'NULL', 1, 0),
    (51, 2, 1731571064, -4.25, -8.37, 0.00, 0.00, 'NULL', 1, 0),
    (52, 2, 348991809, 5.81, -1.32, 0.00, 0.00, 'NULL', 1, 0),
    (53, 2, 73162529, 8.86, -3.77, 0.00, 0.00, 'NULL', 1, 0),
    (54, 2, 1915535094, 5.05, 1.43, 0.00, 0.00, 'NULL', 1, 0),
    (55, 2, 1744550576, 4.70, 5.92, 0.00, 0.00, 'NULL', 1, 0),
    (56, 2, 371024282, -0.91, -9.06, 0.00, 0.00, 'NULL', 1, 0);
INSERT INTO dbo.Asteroids
VALUES
    (76, 3, 1550415097, -1.18, 9.43, 0.00, 0.00, 'NULL', 1, 0),
    (77, 3, 1774162384, 6.82, -4.28, 0.00, 0.00, 'NULL', 1, 0),
    (78, 3, 732505902, 8.33, -2.21, 0.00, 0.00, 'NULL', 1, 0),
    (79, 3, 472385461, 0.18, -8.04, 0.00, 0.00, 'NULL', 1, 0),
    (80, 3, 1896741477, 7.93, -4.08, 0.00, 0.00, 'NULL', 1, 0),
    (81, 3, 497645697, -1.28, 6.56, 0.00, 0.00, 'NULL', 1, 0),
    (82, 3, 1039099833, -4.43, 5.77, 0.00, 0.00, 'NULL', 1, 0),
    (89, 4, 452857552, -2.95, -7.50, 0.00, 0.00, 'NULL', 1, 0),
    (90, 4, 28131997, -5.47, 0.44, 0.00, 0.00, 'NULL', 1, 0);
INSERT INTO dbo.Asteroids
VALUES
    (107, 4, 2024773164, -5.35, -1.08, 0.00, 0.00, 'NULL', 1, 0),
    (112, 5, 1899658258, 0.74, -5.02, 0.00, 0.00, 'NULL', 1, 0),
    (113, 5, 261107231, -8.40, -4.63, 0.00, 0.00, 'NULL', 1, 0),
    (114, 5, 996171722, -5.36, 5.79, 0.00, 0.00, 'NULL', 1, 0),
    (115, 5, 1931437314, -6.36, 1.93, 0.00, 0.00, 'NULL', 1, 0),
    (116, 5, 1319203683, 2.12, -6.13, 0.00, 0.00, 'NULL', 1, 0),
    (117, 5, 384009525, 1.58, -7.21, 0.00, 0.00, 'NULL', 1, 0),
    (118, 5, 2087404879, 0.66, -7.36, 0.00, 0.00, 'NULL', 1, 0),
    (119, 5, 973526939, -8.03, 2.87, 0.00, 0.00, 'NULL', 1, 0);
INSERT INTO dbo.Asteroids
VALUES
    (141, 6, 298709388, -8.01, -0.27, 0.00, 0.00, 'NULL', 1, 0),
    (142, 6, 1337734493, -7.37, 2.10, 0.00, 0.00, 'NULL', 1, 0),
    (143, 6, 1766606504, 7.79, 2.35, 0.00, 0.00, 'NULL', 1, 0),
    (144, 6, 2069336478, -6.98, -0.92, 0.00, 0.00, 'NULL', 1, 0),
    (145, 6, 316111025, 1.11, -6.31, 0.00, 0.00, 'NULL', 1, 0),
    (146, 6, 1933984071, -5.72, 5.98, 0.00, 0.00, 'NULL', 1, 0),
    (147, 6, 369295901, -8.29, -3.93, 0.00, 0.00, 'NULL', 1, 0),
    (148, 6, 1072927745, -4.97, -0.92, 0.00, 0.00, 'NULL', 1, 0),
    (156, 7, 230858351, 6.31, 1.08, 0.00, 0.00, 'NULL', 1, 0);
INSERT INTO dbo.Asteroids
VALUES
    (180, 8, 2115221718, 1.68, 6.44, 0.00, 0.00, 'NULL', 1, 0),
    (181, 8, 1819739528, -6.92, 0.13, 0.00, 0.00, 'NULL', 1, 0),
    (182, 8, 1368735312, 4.83, -3.02, 0.00, 0.00, 'NULL', 1, 0),
    (183, 8, 2144497605, -8.98, -0.09, 0.00, 0.00, 'NULL', 1, 0),
    (184, 8, 581884383, 5.97, 3.01, 0.00, 0.00, 'NULL', 1, 0),
    (185, 8, 711849211, 0.38, 7.18, 0.00, 0.00, 'NULL', 1, 0),
    (186, 8, 618400691, -1.95, -4.69, 0.00, 0.00, 'NULL', 1, 0);

-- api/reset

-- Drop Commands executed by reset
DROP TABLE dbo.Galaxies;
DROP TABLE dbo.Systems;
DROP TABLE dbo.SystemConnections;
DROP TABLE dbo.Planets;
DROP TABLE dbo.Asteroids;
DROP TABLE dbo.Players;
DROP TABLE dbo.Ships;
DROP TABLE dbo.SessionHistory;
DROP TABLE dbo.CombatHistory;
DROP TABLE dbo.Mines;
DROP TABLE dbo.Visits;
-- Create Commands excuted by reset
CREATE TABLE dbo.Galaxies
(
    g_galaxy_id INT PRIMARY KEY,
    g_seed INT NOT NULL
);
CREATE TABLE dbo.Systems
(
    s_system_id INT PRIMARY KEY,
    s_galaxy_id INT NOT NULL,
    s_seed INT NOT NULL,
    s_position_x DECIMAL(4,2) NOT NULL,
    s_position_y DECIMAL(4,2) NOT NULL
);
CREATE TABLE dbo.SystemConnections
(
    sc_system_1_id INT NOT NULL,
    sc_system_2_id INT NOT NULL,
    sc_travel_cost INT NOT NULL,
    PRIMARY KEY (sc_system_1_id, sc_system_2_id)
);
CREATE TABLE dbo.Planets
(
    p_planet_id INT PRIMARY KEY,
    p_system_id INT NOT NULL,
    p_seed INT NOT NULL,
    p_radius DECIMAL(4,2) NOT NULL,
    p_theta DECIMAL(4,2) NOT NULL,
    p_size DECIMAL(4,2) NOT NULL,
    p_density DECIMAL(4,2) NOT NULL,
    p_composition VARCHAR(20) NOT NULL,
    p_is_habitable BIT NOT NULL,
    p_is_inhabited BIT NOT NULL,
    p_kardashev_level DECIMAL(4,2) NOT NULL,
    p_economy_type VARCHAR(100) NOT NULL
);
CREATE TABLE dbo.Asteroids
(
    a_asteroid_id INT PRIMARY KEY,
    a_system_id INT NOT NULL,
    a_seed INT NOT NULL,
    a_radius DECIMAL(4,2) NOT NULL,
    a_theta DECIMAL(4,2) NOT NULL,
    a_size DECIMAL(4,2) NOT NULL,
    a_density DECIMAL(4,2) NOT NULL,
    a_composition VARCHAR(20) NOT NULL,
    a_is_mineable BIT NOT NULL,
    a_is_regenerating BIT NOT NULL
);
CREATE TABLE dbo.Players
(
    py_player_id INT PRIMARY KEY,
    py_current_session INT NOT NULL,
    py_name VARCHAR(40) NOT NULL,
    py_password VARCHAR(40) NOT NULL,
    py_balance INT NOT NULL
);
CREATE TABLE dbo.Ships
(
    sp_ship_id INT PRIMARY KEY,
    sp_player_id INT NOT NULL,
    sp_name VARCHAR(40) NOT NULL,
    sp_data VARCHAR(MAX) NOT NULL,
    sp_position_x DECIMAL(4,2) NOT NULL,
    sp_position_y DECIMAL(4,2) NOT NULL
);
CREATE TABLE dbo.SessionHistory
(
    sh_session_id INT PRIMARY KEY,
    sh_player_id INT NULL,
    sh_login DATETIME NULL,
    sh_logout DATETIME NULL
);
CREATE TABLE dbo.CombatHistory
(
    ch_combat_id INT PRIMARY KEY,
    ch_ship_1_id INT NULL,
    ch_ship_2_id INT NULL,
    ch_date DATETIME NULL
);
CREATE TABLE dbo.Mines
(
    m_ship_id INT NOT NULL,
    m_asteroid_id INT NOT NULL,
    m_amount INT NOT NULL,
    m_date DATETIME NOT NULL,
    PRIMARY KEY (m_ship_id, m_asteroid_id, m_date)
);
CREATE TABLE dbo.Visits
(
    v_ship_id INT NOT NULL,
    v_planet_id INT NOT NULL,
    v_date DATETIME NOT NULL,
    PRIMARY KEY (v_ship_id, v_planet_id, v_date)
);