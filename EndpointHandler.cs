using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace BitNaughts {
    public static class EndpointHandler {

        /* Result-formatting constants */
        public const string DELIMITER = ",";
        public const string NEW_LINE = "\n";

        /* Endpoint Functions */
        /* * * * * * * * * * */
        [FunctionName ("Create")] /* API Endpoints: /api/create?flag=reset, /api/create?flag=add&table=players */
        public static async Task<string> Create ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "create")] HttpRequest req) {
            try {
                /* Reads data into table and returns transaction receipt */
                switch (req.Query["flag"]) {
                    case "reset":

                        /* Pulls galaxy JSON from HTTP Body */
                        dynamic galaxy = await GetBody (req.Body);

                        /* Entity Table Values */
                        string galaxy_values = "";
                        string system_values = "";
                        List<string> planet_values = new List<string> ();
                        List<string> asteroid_values = new List<string> ();

                        /* Relation Table Values */
                        string system_link_values = "";
                        List<string> system_connection_values = new List<string> ();
                        List<string> planet_link_values = new List<string> ();
                        List<string> asteroid_link_values = new List<string> ();

                        /* Agregrates Table Values */
                        galaxy_values = WrapValues (new string[] {
                            galaxy.id, galaxy.seed
                        });
                        system_values = String.Join (DELIMITER, ((IEnumerable<dynamic>) galaxy.systems).Select (
                            system => WrapValues (new string[] {
                                system.id, system.seed, system.position_x, system.position_y
                            })
                        ));
                        system_link_values = String.Join (DELIMITER, ((IEnumerable<dynamic>) galaxy.systems).Select (
                            system => WrapValues (new string[] {
                                galaxy.id, system.id
                            })
                        ));
                        foreach (dynamic system in galaxy.systems) {
                            system_connection_values.Add (String.Join (DELIMITER, ((IEnumerable<dynamic>) system.connected_systems).Select (
                                connected_system => WrapValues (new string[] {
                                    galaxy.id, system.id, connected_system
                                })
                            )));
                            planet_values.Add (String.Join (DELIMITER, ((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    planet.id, planet.seed
                                })
                            )));
                            planet_link_values.Add (String.Join (DELIMITER, ((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    system.id, planet.id
                                })
                            )));
                            asteroid_values.Add (String.Join (DELIMITER, ((IEnumerable<dynamic>) system.asteroids).Select (
                                asteroid => WrapValues (new string[] {
                                    asteroid.id, asteroid.seed, asteroid.size
                                })
                            )));
                            asteroid_link_values.Add (String.Join (DELIMITER, ((IEnumerable<dynamic>) system.asteroids).Select (
                                asteroid => WrapValues (new string[] {
                                    system.id, asteroid.id
                                })
                            )));
                        }

                        /* Execute generated SQL */
                        return ExecuteNonQuery (
                            new string[] {
                                /* Cleaning Entity Tables */
                                "DELETE FROM dbo.Galaxies",
                                "DELETE FROM dbo.Systems",
                                "DELETE FROM dbo.Planets",
                                "DELETE FROM dbo.Asteroids",
                                /* Cleaning Relation Tables */
                                "DELETE FROM dbo.SystemConnections",
                                "DELETE FROM dbo.SystemLinks",
                                "DELETE FROM dbo.PlanetLinks",
                                "DELETE FROM dbo.AsteroidLinks",
                                /* Populating Entity Tables */
                                "INSERT INTO dbo.Galaxies VALUES" + galaxy_values,
                                "INSERT INTO dbo.Systems VALUES" + system_values,
                                "INSERT INTO dbo.Planets VALUES" + String.Join (DELIMITER, planet_values.ToArray ()),
                                "INSERT INTO dbo.Asteroids VALUES" + String.Join (DELIMITER, asteroid_values.ToArray ()),
                                /* Populating Relation Tables */
                                "INSERT INTO dbo.SystemConnections VALUES" + system_connection_values,
                                "INSERT INTO dbo.SystemLinks VALUES" + String.Join (DELIMITER, system_link_values.ToArray ()),
                                "INSERT INTO dbo.PlanetLinks VALUES" + String.Join (DELIMITER, planet_link_values.ToArray ()),
                                "INSERT INTO dbo.AsteroidLinks VALUES" + String.Join (DELIMITER, asteroid_link_values.ToArray ())
                            }
                        );
                    case "add":
                        return "Add not set up yet";
                }
                return "Flag not set...";
            } catch (Exception ex) {
                return ex.ToString ();
            }
        }

        [FunctionName ("Read")] /* API Endpoint: /api/read?table=players&fields=* */
        public static async Task<string> Read ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "read")] HttpRequest req) {
            try {
                /* Returns formatted result of selection query */
                switch (req.Query["flag"]) {
                    case "generic":

                        dynamic req_body = await GetBody (req.Body);
                        return ExecuteQuery (
                            String.Format (
                                "SELECT {1} FROM dbo.{0} WHERE {2}", /* SQL Query to be executed */
                                req.Query["table"],
                                String.Join (
                                    DELIMITER,
                                    req_body.values.ToObject<string[]> ()
                                ),
                                req_body.condition
                            )
                        );
                    case "fun-facts":
                        return ExecuteQuery (
                            "To be determined... complex, fun facts sort of queries to satisfy requirements for DB project"
                        );
                }
                return "Flag not set...";
            } catch (Exception ex) {
                return ex.ToString ();
            }
        }

        [FunctionName ("Update")] /* API Endpoint: /api/update?table=players */
        public static async Task<string> Update ([HttpTrigger (AuthorizationLevel.Anonymous, "put", Route = "update")] HttpRequest req) {

            /* Overrides data in table and returns transaction receipt */
            dynamic req_body = await GetBody (req.Body);
            return ExecuteNonQuery (
                String.Format (
                    "UPDATE dbo.{0} SET {1} WHERE {2}", /* SQL Query to be executed */
                    req.Query["table"],
                    String.Join (
                        DELIMITER,
                        req_body.values.ToObject<string[]> ()
                    ),
                    req_body.condition
                )
            );
        }

        [FunctionName ("Delete")] /* API Endpoint: /api/delete?table=players */
        public static async Task<string> Delete ([HttpTrigger (AuthorizationLevel.Anonymous, "delete", Route = "delete")] HttpRequest req) {

            /* Removes data in table and returns transaction receipt */
            dynamic req_body = await GetBody (req.Body);
            return ExecuteNonQuery (
                String.Format (
                    "DELETE FROM dbo.{0} WHERE {1}", /* SQL Query to be executed */
                    req.Query["table"],
                    req_body.condition
                )
            );
        }

        /* Helper Functions */
        /* * * * * * * * * */

        /* Assembles HTTP Body byte-stream into JSON */
        public static async Task<dynamic> GetBody (Stream body) {
            using (var reader = new StreamReader (body)) {
                string body_stream = await reader.ReadToEndAsync ();
                return Newtonsoft.Json.JsonConvert.DeserializeObject (body_stream);
            }
        }

        public static async Task<Dictionary<string, string>> GetBodyAsDict (HttpRequest req) {
            using (var reader = new StreamReader (req.Body)) {
                string body_stream = await reader.ReadToEndAsync ();
                return ((IEnumerable<KeyValuePair<string, Newtonsoft.Json.Linq.JToken>>) Newtonsoft.Json.JsonConvert.DeserializeObject (body_stream))
                    .ToDictionary (param => param.Key, param => param.Value.ToString ());
            }
        }
        public static string WrapValues (string[] values) {
            return "(" + String.Join (DELIMITER, values) + ")";
        }
        public static IEnumerable<dynamic> GetEnum (dynamic json) {
            IEnumerable<dynamic> enum_json = json;
            return enum_json;
        }

        /* Manages database connection, runs queries, and returns results */
        public static string ExecuteQuery (string[] queries) {
            string result = "";
            foreach (string query in queries) {
            result += ExecuteQuery (query) + "\n";
            }
            return result;
        }
        public static string ExecuteQuery (string query) {
            try {
                /* Defines connection parameters and query logic */
                using (SqlConnection connection = new SqlConnection (System.Environment.GetEnvironmentVariable ("Connection String"))) {
                    using (SqlCommand command = new SqlCommand (query, connection)) {

                        /* Connects to database and executes query */
                        connection.Open ();
                        using (SqlDataReader reader = command.ExecuteReader ()) {

                            /* Holds row results as they are read */
                            List<string> results = new List<string> ();
                            while (reader.Read ()) {

                                /* Dumps values into Object array */
                                Object[] fields = new Object[reader.FieldCount];
                                reader.GetValues (fields);

                                /* Adds row result as delimiter-seperated values */
                                results.Add (
                                    String.Join (
                                        DELIMITER,
                                        fields.Where (x => x != null)
                                        .Select (x => x.ToString ())
                                        .ToArray ()
                                    )
                                );
                            }

                            /* Closes the database connection */
                            reader.Close ();
                            connection.Close ();

                            /* Returns formatted results from query */
                            return String.Join (
                                NEW_LINE,
                                results.ToArray ()
                            );
                        }
                    }
                }
            } catch (Exception ex) {
                return String.Format (
                    "Error({0}): {1}",
                    query.Length > 50 ? query.Substring(0, 50) + "..." : query,
                    ex.ToString ()
                );
            }
        }

        /* Manages database connection, runs commands, and returns receipts */
        public static string ExecuteNonQuery (string[] queries) {
            string result = "";
            foreach (string query in queries) {
                result += ExecuteNonQuery (query) + "\n";
            }
            return result;
        }
        public static string ExecuteNonQuery (string query) {
            try {
                /* Defines connection parameters and query logic */
                using (SqlConnection connection = new SqlConnection (System.Environment.GetEnvironmentVariable ("Connection String"))) {
                    using (SqlCommand command = new SqlCommand (query, connection)) {

                        /* Connects to database and executes non-query */
                        connection.Open ();
                        int rows_modified = command.ExecuteNonQuery ();

                        /* Returns number of rows modified */
                        return String.Format (
                            "Query({0}): {1} Row(s) Modified",
                            query.Length > 50 ? query.Substring(0, 50) + "..." : query,
                            rows_modified
                        );
                    }
                }
            } catch (Exception ex) {
                return String.Format (
                    "Error({0}): {1}",
                    query.Length > 50 ? query.Substring(0, 50) + "..." : query,
                    ex.ToString ()
                );
            };
        }
    }
}