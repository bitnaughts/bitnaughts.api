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