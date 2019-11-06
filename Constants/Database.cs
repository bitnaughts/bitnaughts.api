public static class Database {
    public const string NAME = "bitnaughtsdb";
    public static class Tables {
        public static class Galaxies {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Galaxies] (
                    [g_galaxy_id] INT PRIMARY KEY,
                    [g_seed] INT NULL
                )",
                TABLE_NAME = "dbo.Galaxies",
                ID = "g_galaxy_id",
                SEED = "g_seed";
        }
        public static class Systems {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Systems] (
                    [s_system_id] INT PRIMARY KEY,
                    [s_galaxy_id] INT NULL,
                    [s_seed] INT NULL
                )",
                TABLE_NAME = "dbo.Systems",
                ID = "s_system_id",
                GALAXY_ID = "s_galaxy_id",
                SEED = "s_seed";
        }
        public static class SystemConnections {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[SystemConnections] (
                    [sc_system_1_id] INT NOT NULL,
                    [sc_system_2_id] INT NOT NULL,
                    [sc_travel_cost] INT NULL,
                    PRIMARY KEY ([sc_system_1_id], [sc_system_2_id])
                )",
                TABLE_NAME = "dbo.SystemConnections",
                SYSTEM_START = "sc_system_1_id",
                SYSTEM_END = "sc_system_2_id",
                TRAVEL_COST = "sc_travel_cost";
        }
        public static class Planets {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Planets] (
                    [p_planet_id] INT PRIMARY KEY, 
                    [p_system_id] INT NULL,
                    [p_radius] DECIMAL(5,2) NULL,
                    [p_offset] DECIMAL(5,2) NULL,
                    [p_size] INT NULL,
                    [p_density] INT NULL,
                    [p_composition] VARCHAR(20) NULL,
                    [p_is_habitable] BIT NULL,
                    [p_is_inhabited] BIT NULL,
                    [p_kardashev_level] INT NULL,
                    [p_economy_type] VARCHAR(10) NULL,
                    [p_seed] INT NULL
                )",
                TABLE_NAME = "dbo.Planets",
                ID = "p_planet_id",
                SYSTEM_ID = "p_system_id",
                SEED = "p_seed",
                RADIUS = "p_radius",
                OFFSET = "p_offset",
                SIZE = "p_size",
                DENSITY = "p_density",
                COMPOSITION = "p_composition",
                IS_HABITABLE = "p_is_habitable",
                IS_INHABITED = "p_is_inhabited",
                KARDASHEV_LEVEL = "p_kardashev_level",
                ECONOMY_TYPE = "p_economy_type";
        }
        public static class Asteroids {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Asteroids] (
                    [a_asteroid_id] INT PRIMARY KEY,
                    [a_system_id] INT NULL,
                    [a_radius] DECIMAL(5,2) NULL,
                    [a_offset] DECIMAL(5,2) NULL,
                    [a_size] INT NULL,
                    [a_density] INT NULL,
                    [a_composition] VARCHAR(20) NULL,
                    [a_is_mineable] BIT NULL,
                    [a_is_regenerating] BIT NULL,
                    [a_seed] INT NULL
                )",
                TABLE_NAME = "dbo.Asteroids",
                ID = "a_asteroid_id",
                SYSTEM_ID = "a_system_id",
                SEED = "a_seed",
                RADIUS = "a_radius",
                OFFSET = "a_offset",
                SIZE = "a_size",
                DENSITY = "a_density",
                COMPOSITION = "a_composition",
                IS_MINEABLE = "a_is_mineable",
                IS_REGENERATING = "a_is_regenerating";
        }
        public static class Players {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Players] (
                    [py_player_id] INT PRIMARY KEY,
                    [py_current_session] INT NULL,
                    [py_name] VARCHAR(50) NULL,
                    [py_password] VARCHAR(50) NULL,
                    [py_balance] INT NULL
                )",
                TABLE_NAME = "dbo.Players",
                ID = "py_player_id",
                CURRENT_SESSION = "py_current_session",
                NAME = "py_name",
                BALANCE = "py_balance";
        }
        public static class Ships {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Ships] (
                    [sp_ship_id] INT PRIMARY KEY,
                    [sp_player_id] INT NULL,
                    [sp_name] VARCHAR(50) NULL,
                    [sp_health] INT NULL,
                    [sp_position_x] DECIMAL(5,2) NULL,
                    [sp_position_y] DECIMAL(5,2) NULL
                )",
                TABLE_NAME = "dbo.Ships",
                ID = "sp_ship_id",
                PLAYER_ID = "sp_player_id",
                CURRENT_SYSTEM = "sp_current_system",
                NAME = "sp_name",
                HEALTH = "sp_health",
                POSITION_X = "sp_position_x",
                POSITION_Y = "sp_position_y";
        }
        public static class SessionHistory {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[SessionHistory] (
                    [sh_session_id] INT PRIMARY KEY,
                    [sh_player_id] INT NULL,
                    [sh_login] DATETIME NULL,
                    [sh_logout] DATETIME NULL
                )",
                TABLE_NAME = "dbo.SessionHistory",
                ID = "sh_session_id",
                PLAYER_ID = "sh_player_id",
                LOG_IN_DATE = "sh_login",
                LOG_OUT_DATE = "sh_logout";
        }
        public static class CombatHistory {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[CombatHistory] (
                    [ch_combat_id] INT PRIMARY KEY,
                    [ch_ship_1_id] INT NULL,
                    [ch_ship_2_id] INT NULL,
                    [ch_date] DATETIME NULL
                )",
                TABLE_NAME = "dbo.CombatHistory",
                ID = "ch_combat_id",
                SHIP_AGGRESSOR = "ch_ship_1_id",
                SHIP_DEFENDER = "ch_ship_2_id",
                DATE = "ch_date";
        }
        public static class Mines {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Mines] (
                    [m_ship_id] INT NOT NULL,
                    [m_asteroid_id] INT NOT NULL,
                    [m_amount] INT NOT NULL,
                    [m_date] DATETIME NOT NULL,
                    PRIMARY KEY ([m_ship_id], [m_asteroid_id], [m_date])
                )",
                TABLE_NAME = "dbo.Mines",
                SHIP_ID = "m_ship_id",
                ASTEROID_ID = "m_asteroid_id",
                AMOUNT = "m_amount",
                DATE = "m_date";
        }
        public static class Visits {
            public const string SQL_DEFINITION =
                @"CREATE TABLE [dbo].[Visits] (
                    [v_ship_id] INT NOT NULL, 
                    [v_planet_id] INT NOT NULL,
                    [v_date] DATETIME NOT NULL,
                    PRIMARY KEY ([v_ship_id], [v_planet_id], [v_date])
                )",
                TABLE_NAME = "dbo.Visits",
                SHIP_ID = "v_ship_id",
                PLANET_ID = "v_planet_id",
                DATE = "v_date";
        }
    }
}

public static class Warehouse {
    public const string SQL_DEFINITION =
        @"CREATE TABLE warehouse (
            w_warehousekey decimal(3,0) not null,
            w_name char(25) not null,
            w_supplierkey decimal(2,0) not null,
            w_capacity decimal(6,2) not null,
            w_address varchar(40) not null,
            w_nationkey decimal(2,0) not null
        )";
}