public static class Database {
    public const string NAME = "bitnaughtsdb";
    public static class TableNames {
        public const string GALAXIES = "dbo.Galaxies",
            SYSTEMS = "dbo.Systems",
            SYSTEM_CONNECTIONS = "dbo.SystemConnections",
            PLANETS = "dbo.Planets",
            ASTEROIDS = "dbo.Asteroids";
    }
    public static class Tables {
        public static class Galaxies {
            public const string ID = "g_galaxy_id",
                SEED = "g_seed";
        }
        public static class Systems {
            public const string ID = "s_system_id",
                GALAXY_ID = "s_galaxy_id",
                SEED = "s_seed";
        }
        public static class SystemConnections {
            public const string SYSTEM_START = "sc_system_1_id",
                SYSTEM_END = "sc_system_2_id",
                TRAVEL_COST = "sc_travel_cost";
        }
        public static class Planets {
            public const string ID = "p_planet_id",
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
            public const string ID = "a_asteroid_id",
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
            public const string ID = "py_player_id",
                CURRENT_SESSION = "py_current_session",
                NAME = "py_name",
                BALANCE = "py_balance";
        }
        public static class Ships {
            public const string ID = "sp_ship_id",
                PLAYER_ID = "sp_player_id",
                CURRENT_SYSTEM = "sp_current_system",
                NAME = "sp_name",
                HEALTH = "sp_health",
                POSITION_X = "sp_position_x",
                POSITION_Y = "sp_position_y";
        }
        public static class SessionHistory {
            public const string ID = "sh_session_id",
                PLAYER_ID = "sh_player_id",
                LOG_IN_DATE = "sh_login",
                LOG_OUT_DATE = "sh_logout";
        }
        public static class CombatHistory {
            public const string ID = "ch_combat_id",
                SHIP_AGGRESSOR = "ch_ship_1_id",
                SHIP_DEFENDER = "ch_ship_2_id",
                DATE = "ch_date";
        }
        public static class Mines {
            public const string SHIP_ID = "m_ship_id",
                ASTEROID_ID = "m_asteroid_id",
                AMOUNT = "m_amount",
                DATE = "m_date";
        }
        public static class Visits {
            public const string SHIP_ID = "v_ship_id",
                PLANET_ID = "v_planet_id",
                DATE = "v_date";
        }
    }
}