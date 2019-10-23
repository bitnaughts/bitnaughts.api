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

        /*  */

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

                        /* Initializing Table Values */
                        List<string> galaxy_values = new List<string> (),
                            system_values = new List<string> (),
                            system_link_values = new List<string> (),
                            system_connection_values = new List<string> (),
                            planet_values = new List<string> (),
                            planet_link_values = new List<string> (),
                            asteroid_values = new List<string> (),
                            asteroid_link_values = new List<string> ();

                        /* Agregrates Table Values */
                        galaxy_values.Add (WrapValues (new string[] {
                            galaxy.id, galaxy.seed
                        }));
                        system_values.Add (String.Join (DELIMITER, ((IEnumerable<dynamic>) galaxy.systems).Select (
                            system => WrapValues (new string[] {
                                system.id, system.seed, system.position_x, system.position_y
                            })
                        )));
                        system_link_values.Add (String.Join (DELIMITER, ((IEnumerable<dynamic>) galaxy.systems).Select (
                            system => WrapValues (new string[] {
                                galaxy.id, system.id
                            })
                        )));
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
                        return SQLHandler.DeleteFrom (new Dictionary<string, string> { 
                            { SQLHandler.GALAXIES, SQLHandler.ALL },
                            { SQLHandler.SYSTEMS, SQLHandler.ALL },
                            { SQLHandler.PLANETS, SQLHandler.ALL },
                            { SQLHandler.ASTEROIDS, SQLHandler.ALL },
                            { SQLHandler.SYSTEM_CONNECTIONS, SQLHandler.ALL },
                            { SQLHandler.SYSTEM_LINKS, SQLHandler.ALL },
                            { SQLHandler.PLANET_LINKS, SQLHandler.ALL },
                            { SQLHandler.ASTEROID_LINKS, SQLHandler.ALL }
                        }) + SQLHandler.InsertInto (new Dictionary<string, List<string>> { 
                            { SQLHandler.GALAXIES, galaxy_values },
                            { SQLHandler.SYSTEMS, system_values },
                            { SQLHandler.PLANETS, planet_values },
                            { SQLHandler.ASTEROIDS, asteroid_values },
                            { SQLHandler.SYSTEM_CONNECTIONS, system_connection_values },
                            { SQLHandler.SYSTEM_LINKS, system_link_values },
                            { SQLHandler.PLANET_LINKS, planet_link_values },
                            { SQLHandler.ASTEROID_LINKS, asteroid_link_values }
                        });
                }
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return "NULL?";
        }

        [FunctionName ("Read")] /* API Endpoint: /api/read?table=players&fields=* */
        public static async Task<string> Read ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "read")] HttpRequest req) {
            // try {
            //     /* Returns formatted result of selection query */
            //     switch (req.Query["flag"]) {
            //         case "generic":

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
            // } catch (Exception ex) {
            //     return ex.ToString ();
            // }
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