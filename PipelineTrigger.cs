using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BitNaughts {
    public static class FunctionApp {

        public const string DELIMITER = ",";
        public const string NEW_LINE = "\n";
        public const string ERROR_MESSAGE = "ERROR";

        /* Database Schema:
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Galaxies(g_galaxy_id, g_seed)                               *
         * SystemLinks(sl_galaxy_id, sl_system_id)                     *
         * Systems(s_system_id, s_seed)                                *
         * PlanetLinks(pl_system_id, pl_planet_id)                     *
         * Planets(p_planet_id, p_seed)                                *
         * AsteroidLinks(al_system_id, al_asteroid_id)                 *
         * Asteroids(a_asteroid_id, a_size, a_seed)                    *
         * Players(py_player_id, py_name, py_password)                 *
         * Owns(o_player_id, o_ship_id)                                *
         * Ships(sh_ship_id, sh_name, sh_position_x, sh_position_y)    *
         * FightAt(fa_ship_1_id, fa_ship_2_id, fa_system_id, fa_date)  *
         * Visits(v_ship_id, v_planet_id, v_date)                      *
         * Mines(m_ship_id, m_asteroid_id, m_amount, m_date)           *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *         
         */

        /* GET ENDPOINTS */
        [FunctionName ("GetPlayers")] /* API Endpoint: /api/get/players */
        public static async Task<string> GetPlayers ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "get/players")] HttpRequest req, ILogger log) {

            /* Returning result of query */
            return String.Join (
                NEW_LINE,
                QueryHandler.ExecuteQuery (
                    /* SQL Query to be executed */
                    "SELECT alias FROM dbo.Players"
                )
            );
        }

        /* ADD ENDPOINTS */
        [FunctionName ("AddGalaxy")] /* API Endpoint: /api/add/planet */
        public static async Task<string> AddGalaxy ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "add/galaxy")] HttpRequest req, ILogger log) {

            string data = await req.Content.ReadAsStringAsync();
            // Here you can process json into an object
            // dynamic parsed = JsonConvert.DeserializeObject(data);

            /* Returning result of query */
            return data + req.Query["id"] + req.Query["seed"] + String.Join (
                NEW_LINE,
                QueryHandler.ExecuteNonQuery (
                    /* SQL Query to be executed */
                    String.Format (
                        "INSERT INTO dbo.Galaxies VALUES ({0}, {1})",
                        req.Query["id"],
                        req.Query["seed"]
                    )
                )
            );
        }

        [FunctionName ("AddSystem")] /* API Endpoint: /api/add/planet */
        public static async Task<string> AddSystem ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "add/system")] HttpRequest req, ILogger log) {

            /* Returning result of query */
            return String.Join (
                NEW_LINE,
                QueryHandler.ExecuteNonQuery (
                    /* SQL Query to be executed */
                    String.Format (
                        "INSERT INTO dbo.Systems VALUES ({0}, {1})",
                        req.Query["id"],
                        req.Query["seed"]
                    )
                )
            );
        }

        [FunctionName ("AddPlanet")] /* API Endpoint: /api/add/planet */
        public static async Task<string> AddPlanet ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "add/planet")] HttpRequest req, ILogger log) {

            /* Returning result of query */
            return String.Join (
                NEW_LINE,
                QueryHandler.ExecuteNonQuery (
                    /* SQL Query to be executed */
                    String.Format (
                        "INSERT INTO dbo.Planets VALUES ({0}, {1})",
                        req.Query["id"],
                        req.Query["seed"]
                    )
                )
            );
        }

        [FunctionName ("AddAsteroid")] /* API Endpoint: /api/add/asteroid */
        public static async Task<string> AddAsteroid ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "add/asteroid")] HttpRequest req, ILogger log) {

            /* Returning result of query */
            return String.Join (
                NEW_LINE,
                QueryHandler.ExecuteNonQuery (
                    /* SQL Query to be executed */
                    String.Format (
                        "INSERT INTO dbo.Asteroids VALUES ({0}, {1}, {2})",
                        req.Query["id"],
                        req.Query["seed"],
                        100
                    )
                )
            );
        }

        [FunctionName ("AddShip")] /* API Endpoint: /api/add/ship */
        public static async Task<string> AddShip ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "add/ship")] HttpRequest req, ILogger log) {

            /* Returning result of query */
            return String.Join (
                NEW_LINE,
                QueryHandler.ExecuteNonQuery (
                    /* SQL Query to be executed */
                    String.Format (
                        "INSERT INTO dbo.Ships VALUES ({0}, {1}, {2}, {3})",
                        req.Query["id"],
                        req.Query["seed"],
                        0,
                        0
                    )
                )
            );
        }

        [FunctionName ("Testing")] /* API Endpoint: /api/Testing?q=query */
        public static async Task<string> Testing ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "Testing")] HttpRequest req, ILogger log) {

            /* Returning result of query */
            return String.Join (
                NEW_LINE,
                QueryHandler.ExecuteQuery (
                    /* SQL Query to be executed */
                    req.Query["q"]
                )
            );
        }

    }
}