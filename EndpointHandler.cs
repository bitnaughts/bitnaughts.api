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

        /* Endpoint Functions */
        /* * * * * * * * * * */
        [FunctionName (Endpoints.SET)] /* API Endpoints: /api/set?flag=reset, /api/set?flag=add&table=players */
        public static async Task<string> Set ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = Endpoints.SET)] HttpRequest req) {
            try {
                /* Reads data into table and returns transaction receipt */
                switch (req.Query[Endpoints.Parameters.FLAG]) {
                    case Endpoints.Parameters.Values.RESET:

                        /* Pulls galaxy JSON from HTTP Body */
                        dynamic galaxy = await GetBody (req.Body);

                        /* Initializing Table Values */
                        Dictionary<string, List<string>> values = new Dictionary<string, List<string>> { { Database.TableNames.GALAXIES, new List<string> () },
                            { Database.TableNames.SYSTEMS, new List<string> () },
                            { Database.TableNames.SYSTEM_CONNECTIONS, new List<string> () },
                            { Database.TableNames.PLANETS, new List<string> () },
                            { Database.TableNames.ASTEROIDS, new List<string> () }
                        };

                        /* Agregrates Table Values */
                        values[Database.TableNames.GALAXIES].Add (WrapValues (new string[] {
                            galaxy.id, galaxy.seed
                        }));
                        foreach (dynamic system in galaxy.systems) {
                            values[Database.TableNames.SYSTEMS].Add (WrapValues (new string[] {
                                system.id, galaxy.id, system.seed, system.position_x, system.position_y
                            }));
                            values[Database.TableNames.SYSTEM_CONNECTIONS].AddRange (((IEnumerable<dynamic>) system.connected_systems).Select (
                                connected_system => WrapValues (new string[] {
                                    system.id, connected_system
                                })
                            ));
                            values[Database.TableNames.PLANETS].AddRange (((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    planet.id, system.id, planet.seed
                                })
                            ));
                            values[Database.TableNames.ASTEROIDS].AddRange (((IEnumerable<dynamic>) system.asteroids).Select (
                                asteroid => WrapValues (new string[] {
                                    asteroid.id, system.id, asteroid.seed, asteroid.size
                                })
                            ));
                        }
                        return SQLHandler.Delete (values.ToDictionary (value => value.Key, value => SQL.ALL)) + /* For every table referenced, clear all existing values */
                            SQLHandler.Insert (values); /* For every table referenced, inject values */
                }
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName (Endpoints.GET)] /* API Endpoint: /api/get?table=players&fields=* */
        public static async Task<string> Get ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = Endpoints.GET)] HttpRequest req) {
            try {
                /* Returns formatted result of selection query */


                string type = req.Query[Endpoints.Parameters.TYPE];
                string id = req.Query[Endpoints.Parameters.ID];

                switch (type) {
                    case Database.TableNames.GALAXIES:

                        // string galaxy_serialized = 

                        SQLHandler.Select (new Dictionary<string, string> { { SQL.COLUMNS, SQL.ALL },
                            { SQL.TABLE, Database.TableNames.GALAXIES },
                            { SQL.CONDITION, Database.Tables.Galaxies.ID + SQL.EQUALS + id }
                        });

                        //Will also likely want 
                        //All System's position_x, position_y
                        //All Systems' links

                        return String.Format ("{id:1, seed:99, systems: { id:1, seed:99, connected_systems: {1, 2, 3, 4}, position_x: 123, position_y: 234 } }");
                    case "fun-facts":
                        return "to be implemented";
                        // return ExecuteQuery (
                        // "To be determined... complex, fun facts sort of queries to satisfy requirements for DB project"
                        // );
                }
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName (Endpoints.UPDATE)] /* API Endpoint: /api/update?table=players */
        public static async Task<string> Update ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.PUT, Route = Endpoints.UPDATE)] HttpRequest req) {

            // /* Overrides data in table and returns transaction receipt */
            // dynamic req_body = await GetBody (req.Body);
            // return ExecuteNonQuery (
            //     String.Format (
            //         "UPDATE dbo.{0} SET {1} WHERE {2}", /* SQL Query to be executed */
            //         req.Query["table"],
            //         String.Join (
            //             FileFormat.DELIMITER,
            //             req_body.values.ToObject<string[]> ()
            //         ),
            //         req_body.condition
            //     )
            // );
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Delete")] /* API Endpoint: /api/delete?table=players */
        public static async Task<string> Delete ([HttpTrigger (AuthorizationLevel.Anonymous, "delete", Route = "delete")] HttpRequest req) {

            // /* Removes data in table and returns transaction receipt */
            // dynamic req_body = await GetBody (req.Body);
            // return ExecuteNonQuery (
            //     String.Format (
            //         "DELETE FROM dbo.{0} WHERE {1}", /* SQL Query to be executed */
            //         req.Query["table"],
            //         req_body.condition
            //     )
            // );
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName (Endpoints.RESET)] /* API Endpoint: /api/reset */
        public static async Task<string> Reset ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = Endpoints.RESET)] HttpRequest req) {
            try {
                /* Force drops all existing tables and regenerates them per the Constructor/*.sql definitions */
                List<string> tables = new List<string> ();
                List<string> tables_data = new List<string> ();

                foreach (string sql_table_path in SQLHandler.GetSQLTableDefinitions ()) {

                    /* Isolates table name from table path and adding to list */
                    tables.Add (
                        sql_table_path.Substring (
                            Math.Max (
                                sql_table_path.LastIndexOf ("/"),
                                sql_table_path.LastIndexOf ("\\")
                            ) + 1
                        ).Split (new string[] { FileFormat.SQL_FILE_TYPE }, StringSplitOptions.None) [0]
                    );

                    /* Reads table files' content into list */
                    tables_data.Add (
                        File.ReadAllText (sql_table_path)
                    );
                }
                /* Drops old tables and creates new ones with updated fields */
                return SQLHandler.Drop (tables) + SQLHandler.Create (tables_data);

            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        /* Helper Functions */
        /* * * * * * * * * */

        /* Assembles HTTP Body byte-stream into JSON */
        public static async Task<dynamic> GetBody (Stream body) {
            using (var reader = new StreamReader (body)) {
                return Newtonsoft.Json.JsonConvert.DeserializeObject (await reader.ReadToEndAsync ());
            }
        }

        /* Abstracting SQL Values to string array  */
        public static string WrapValues (string[] values) {
            return "(" + String.Join (FileFormat.DELIMITER, values) + ")";
        }
    }
}