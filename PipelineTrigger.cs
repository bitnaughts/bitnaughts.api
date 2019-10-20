using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Nancy.Json;

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

        public static async Task<dynamic> GetBody (HttpRequest req) {
            string requestBody = await new StreamReader (req.Body).ReadToEndAsync ();
            return Newtonsoft.Json.JsonConvert.DeserializeObject (requestBody);
        }
        public static string InsertIntoTable (string table, string[] values) {
            return QueryHandler.ExecuteNonQuery (
                /* SQL Query to be executed */
                String.Format (
                    "INSERT INTO dbo.{0} VALUES ({1})",
                    table,
                    String.Join (
                        DELIMITER,
                        values
                    )
                )
            );
        }

        /* GET ENDPOINT */
        [FunctionName ("Get")] /* API Endpoint: /api/get?Table=players */
        public static async Task<string> Get ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "get")] HttpRequest req, ILogger log) {

            /* Returning CSV formatted result of query */
            return QueryHandler.ExecuteQuery (
                String.Format (
                    "SELECT * FROM dbo.{0}", /* SQL Query to be executed */
                    req.Query["table"]
                )
            );
        }

        /* ADD ENDPOINT */
        [FunctionName ("Add")] /* API Endpoint: /api/add?Table=players */
        public static async Task<string> Add ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "add")] HttpRequest req, ILogger log) {

            /* Reading data into table and returning transaction receipt */
            dynamic req_body = await GetBody (req);
            return QueryHandler.ExecuteNonQuery (
                String.Format (
                    "INSERT INTO dbo.{0} VALUES ({1})", /* SQL Query to be executed */
                    req.Query["table"],
                    String.Join (
                        DELIMITER,
                        req_body.values.ToObject<string[]> ()
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