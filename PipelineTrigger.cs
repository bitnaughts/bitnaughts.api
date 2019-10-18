using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace BitNaughts {
    public static class FunctionApp {

        /* Flag for Error Messages from database connection or execution */
        public const string ERROR_MESSAGE = "ERROR";

        /* Helper Function for managing database connection, running commands, and returning results */
        public static string[] ExecuteQuery (string query) {
            try {
                /* Defines connection parameters and query logic */
                SqlConnection connection = new SqlConnection (System.Environment.GetEnvironmentVariable ("Connection String"));
                SqlCommand command = new SqlCommand (query, connection);

                /* Connects to database and executes query */
                connection.Open ();
                SqlDataReader reader = command.ExecuteReader ();

                /* Stores the returned rows into an Object array */
                Object[] rows = new Object[reader.FieldCount];
                int fieldCount = reader.GetValues (rows);

                /* Closes the database connection */
                reader.Close ();
                connection.Close ();

                /* Return rows as strings */
                return (string[])rows;

            } catch (Exception ex) {
                return new string[] { ERROR_MESSAGE, ex.ToString () };
            }
        }

        [FunctionName ("GetPlayers")] /* API Endpoint: /api/GetPlayers?db=database-name */
        public static async Task<string> GetPlayers ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "GetPlayers")] HttpRequest req, ILogger log) {

            /* Returning result of query */
            return String.Join (

                /* Result row separator */
                "\n",
                ExecuteQuery (
                    String.Format (
                        
                        /* SQL Query to be executed */
                        "SELECT alias FROM {0}",
                        req.Query["db"]
                    )
                )
            );
        }



    }
}