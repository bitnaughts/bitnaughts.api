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
        public const string DELIMITER = ",",
            NEW_LINE = "\n";

        /* Endpoints */
        public const string GET = "get",
            SET = "set";

        /* Operation Types */
        public const string HTTP_GET = "get",
            HTTP_POST = "post";

        /* Endpoint Constants (Also see DatabaseHandler.cs) */
        public const string FLAG = "flag", //These constants should be consolidated into a shared class between the front and back ends...
            RESET = "reset", //These constants should be consolidated into a shared class between the front and back ends...
            TYPE = "type", //These constants should be consolidated into a shared class between the front and back ends...
            ID = "id"; //These constants should be consolidated into a shared class between the front and back ends...


        /* Class Types */
        public const string GALAXY_OBJECT = "GalaxyObject";

        /* Endpoint Functions */
        /* * * * * * * * * * */
        [FunctionName (SET)] /* API Endpoints: /api/set?flag=reset, /api/set?flag=add&table=players */
        public static async Task<string> Set ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP_POST, Route = SET)] HttpRequest req) {
            try {
                /* Reads data into table and returns transaction receipt */
                switch (req.Query[FLAG]) {
                    case RESET:

                        /* Pulls galaxy JSON from HTTP Body */
                        dynamic galaxy = await GetBody (req.Body);

                        /* Initializing Table Values */
                        Dictionary<string, List<string>> values = new Dictionary<string, List<string>> { { SQLHandler.GALAXIES, new List<string> () },
                            { SQLHandler.SYSTEMS, new List<string> () },
                            { SQLHandler.PLANETS, new List<string> () },
                            { SQLHandler.ASTEROIDS, new List<string> () },
                            { SQLHandler.SYSTEM_CONNECTIONS, new List<string> () },
                            { SQLHandler.SYSTEM_LINKS, new List<string> () },
                            { SQLHandler.PLANET_LINKS, new List<string> () },
                            { SQLHandler.ASTEROID_LINKS, new List<string> () }
                        };

                        /* Agregrates Table Values */
                        values[SQLHandler.GALAXIES].Add (WrapValues (new string[] {
                            galaxy.id, galaxy.seed
                        }));
                        foreach (dynamic system in galaxy.systems) {
                            values[SQLHandler.SYSTEMS].Add (WrapValues (new string[] {
                                system.id, system.seed, system.position_x, system.position_y
                            }));
                            values[SQLHandler.SYSTEM_LINKS].Add (WrapValues (new string[] {
                                galaxy.id, system.id
                            }));
                            values[SQLHandler.SYSTEM_CONNECTIONS].AddRange (((IEnumerable<dynamic>) system.connected_systems).Select (
                                connected_system => WrapValues (new string[] {
                                    system.id, connected_system
                                })
                            ));
                            values[SQLHandler.PLANETS].AddRange (((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    planet.id, planet.seed
                                })
                            ));
                            values[SQLHandler.PLANET_LINKS].AddRange (((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    system.id, planet.id
                                })
                            ));
                            values[SQLHandler.ASTEROIDS].AddRange (((IEnumerable<dynamic>) system.asteroids).Select (
                                asteroid => WrapValues (new string[] {
                                    asteroid.id, asteroid.seed, asteroid.size
                                })
                            ));
                            values[SQLHandler.ASTEROID_LINKS].AddRange (((IEnumerable<dynamic>) system.asteroids).Select (
                                asteroid => WrapValues (new string[] {
                                    system.id, asteroid.id
                                })
                            ));
                        }
                        return SQLHandler.Delete (values.ToDictionary (value => value.Key, value => SQLHandler.ALL)) + /* For every table referenced, clear all existing values */
                            SQLHandler.Insert (values); /* For every table referenced, inject values */
                }
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return "NULL?";
        }

        [FunctionName (GET)] /* API Endpoint: /api/get?table=players&fields=* */
        public static async Task<string> Get ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP_GET, Route = GET)] HttpRequest req) {
            try {
                /* Returns formatted result of selection query */
                switch (req.Query[TYPE]) {
                    case GALAXY_OBJECT:
                        return SQLHandler.Select(new Dictionary<string, string>{
                            {SQLHandler.COLUMNS, SQLHandler.ALL},
                            {SQLHandler.TABLE, SQLHandler.GALAXIES},
                            {SQLHandler.CONDITION, "g_galaxy_id = " + req.Query[ID]} //We will want constants class abstract for this sort of class of wanting Galaxies' "g_galaxy_id" wrapped 
                        });
                        // string id = req.Query[ID];
                        // 
                }

            //             dynamic req_body = await GetBody (req.Body);
            //             return ExecuteQuery (
            //                 String.Format (
            //                     "SELECT {1} FROM dbo.{0} WHERE {2}", /* SQL Query to be executed */
            //                     req.Query["table"],
            //                     String.Join (
            //                         DELIMITER,
            //                         req_body.values.ToObject<string[]> ()
            //                     ),
            //                     req_body.condition
            //                 )
            //             );
            //         case "fun-facts":
            //             return ExecuteQuery (
            //                 "To be determined... complex, fun facts sort of queries to satisfy requirements for DB project"
            //             );
            //     }
            //     return "Flag not set...";
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return "to be implemented";
        }

        [FunctionName ("Update")] /* API Endpoint: /api/update?table=players */
        public static async Task<string> Update ([HttpTrigger (AuthorizationLevel.Anonymous, "put", Route = "update")] HttpRequest req) {

            // /* Overrides data in table and returns transaction receipt */
            // dynamic req_body = await GetBody (req.Body);
            // return ExecuteNonQuery (
            //     String.Format (
            //         "UPDATE dbo.{0} SET {1} WHERE {2}", /* SQL Query to be executed */
            //         req.Query["table"],
            //         String.Join (
            //             DELIMITER,
            //             req_body.values.ToObject<string[]> ()
            //         ),
            //         req_body.condition
            //     )
            // );
            return "to be implemented";
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
            return "to be implemented";
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
            return "(" + String.Join (DELIMITER, values) + ")";
        }
    }
}