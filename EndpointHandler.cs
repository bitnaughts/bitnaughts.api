using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace BitNaughts {
    public static class EndpointHandler {

        [FunctionName ("Lab7CreatewarehouseTable")] /* API Endpoints: /api/Lab7CreatewarehouseTable */
        public static async Task<string> Lab7CreatewarehouseTable ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7CreatewarehouseTable")] HttpRequest req) {
            try {
                SQLHandler.ExecuteSQLiteNonQuery (Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }
        [FunctionName ("Lab7Createwarehouse")] /* API Endpoints: /api/Lab7Createwarehouse */
        public static async Task<string> Lab7Createwarehouse ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7Createwarehouse")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                //SQLHandler.ExecuteSQLNonQuery(Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }
        [FunctionName ("Lab7MinWarehouseSupplier")] /* API Endpoints: /api/Lab7MinWarehouseSupplier */
        public static async Task<string> Lab7MinWarehouseSupplier ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7MinWarehouseSupplier")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                //SQLHandler.ExecuteSQLNonQuery(Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }
        [FunctionName ("Lab7MaxWarehouseCapacity")] /* API Endpoints: /api/Lab7MaxWarehouseCapacity */
        public static async Task<string> Lab7MaxWarehouseCapacity ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7MaxWarehouseCapacity")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                //SQLHandler.ExecuteSQLNonQuery(Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }
        [FunctionName ("Lab7EuropeanWarehousesSmallerThanX")] /* API Endpoints: /api/Lab7EuropeanWarehousesSmallerThanX */
        public static async Task<string> Lab7EuropeanWarehousesSmallerThanX ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7EuropeanWarehousesSmallerThanX")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                //SQLHandler.ExecuteSQLNonQuery(Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }
        [FunctionName ("Lab7WarehouseLargeEnoughForSupplier")] /* API Endpoints: /api/Lab7WarehouseLargeEnoughForSupplier */
        public static async Task<string> Lab7WarehouseLargeEnoughForSupplier ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7WarehouseLargeEnoughForSupplier")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                //SQLHandler.ExecuteSQLNonQuery(Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }
        [FunctionName ("Lab7WarehousesInNation")] /* API Endpoints: /api/Lab7WarehousesInNation */
        public static async Task<string> Lab7WarehousesInNation ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7WarehousesInNation")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                //SQLHandler.ExecuteSQLNonQuery(Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }
        [FunctionName ("Lab7WarehouseChange")] /* API Endpoints: /api/Lab7WarehouseChange */
        public static async Task<string> Lab7WarehouseChange ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7WarehouseChange")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                //SQLHandler.ExecuteSQLNonQuery(Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

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
                        Dictionary<string, List<string>> values = new Dictionary<string, List<string>> { { Database.Tables.Galaxies.TABLE_NAME, new List<string> () },
                            { Database.Tables.Systems.TABLE_NAME, new List<string> () },
                            { Database.Tables.SystemConnections.TABLE_NAME, new List<string> () },
                            { Database.Tables.Planets.TABLE_NAME, new List<string> () },
                            { Database.Tables.Asteroids.TABLE_NAME, new List<string> () }
                        };

                        /* Agregrates Table Values */
                        values[Database.Tables.Galaxies.TABLE_NAME].Add (WrapValues (new string[] {
                            galaxy.id, galaxy.seed
                        }));
                        foreach (dynamic system in galaxy.systems) {
                            values[Database.Tables.Systems.TABLE_NAME].Add (WrapValues (new string[] {
                                system.id, galaxy.id, system.seed, system.position_x, system.position_y
                            }));
                            values[Database.Tables.SystemConnections.TABLE_NAME].AddRange (((IEnumerable<dynamic>) system.connected_systems).Select (
                                connected_system => WrapValues (new string[] {
                                    system.id, connected_system
                                })
                            ));
                            values[Database.Tables.Planets.TABLE_NAME].AddRange (((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    planet.id, system.id, planet.seed
                                })
                            ));
                            values[Database.Tables.Asteroids.TABLE_NAME].AddRange (((IEnumerable<dynamic>) system.asteroids).Select (
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
                    case Database.Tables.Galaxies.TABLE_NAME:

                        // string galaxy_serialized = 

                        SQLHandler.Select (new Dictionary<string, string> { { SQL.COLUMNS, SQL.ALL },
                            { SQL.TABLE, Database.Tables.Galaxies.TABLE_NAME },
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
                /* Drops old tables and creates new ones with updated fields */
                return SQLHandler.Drop (
                    new string[] {
                        Database.Tables.Galaxies.TABLE_NAME,
                            Database.Tables.Systems.TABLE_NAME,
                            Database.Tables.SystemConnections.TABLE_NAME,
                            Database.Tables.Planets.TABLE_NAME,
                            Database.Tables.Asteroids.TABLE_NAME,
                            Database.Tables.Ships.TABLE_NAME,
                            Database.Tables.SessionHistory.TABLE_NAME,
                            Database.Tables.CombatHistory.TABLE_NAME,
                            Database.Tables.Mines.TABLE_NAME,
                            Database.Tables.Visits.TABLE_NAME
                    }
                ) + SQLHandler.Create (
                    new string[] {
                        Database.Tables.Galaxies.SQL_DEFINITION,
                            Database.Tables.Systems.SQL_DEFINITION,
                            Database.Tables.SystemConnections.SQL_DEFINITION,
                            Database.Tables.Planets.SQL_DEFINITION,
                            Database.Tables.Asteroids.SQL_DEFINITION,
                            Database.Tables.Ships.SQL_DEFINITION,
                            Database.Tables.SessionHistory.SQL_DEFINITION,
                            Database.Tables.CombatHistory.SQL_DEFINITION,
                            Database.Tables.Mines.SQL_DEFINITION,
                            Database.Tables.Visits.SQL_DEFINITION
                    }
                );
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
            return "(" + String.Join (SQL.Format.DELIMITER, values) + ")";
        }
    }
}