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

        [FunctionName ("Lab7CreateWarehouseTable")] /* API Endpoints: /api/Lab7CreateWarehouseTable */
        public static async Task<string> Lab7CreateWarehouseTable ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7CreateWarehouseTable")] HttpRequest req) {
            try {
                return SQLHandler.ExecuteSQLiteNonQuery ("DROP TABLE warehouse") +
                    SQLHandler.ExecuteSQLiteNonQuery (Warehouse.SQL_DEFINITION);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Lab7CreateWarehouse")] /* API Endpoints: /api/Lab7CreateWarehouse */
        public static async Task<string> Lab7CreateWarehouse ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = "Lab7CreateWarehouse")] HttpRequest req) {
            try {
                dynamic warehouse = await GetBody (req.Body);

                string supp_key = SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT s_suppkey
                    FROM supplier
                    WHERE s_name = '" + (string) warehouse.supplier + "'");
                string nat_key = SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT n_nationkey
                    FROM nation
                    WHERE n_name = '" + (string) warehouse.nation + "'");
                string warehouse_index = SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT COUNT(*)
                    FROM warehouse");
                return "Added warehouse: " + SQLHandler.ExecuteSQLiteNonQuery (
                    String.Format (
                        "INSERT INTO warehouse VALUES ({0}, '{1}', {2}, {3}, '{4}', {5})",
                        warehouse_index,
                        (string) warehouse.name,
                        supp_key,
                        (string) warehouse.capacity,
                        (string) warehouse.address,
                        nat_key
                    )
                );
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Lab7MinWarehouseSupplier")] /* API Endpoints: /api/Lab7MinWarehouseSupplier */
        public static async Task<string> Lab7MinWarehouseSupplier ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = "Lab7MinWarehouseSupplier")] HttpRequest req) {
            try {
                if (int.Parse (SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT COUNT(*)
                    FROM supplier
                    LEFT JOIN (
                        SELECT w_supplierkey, COUNT(*) sum_warehouses
                        FROM warehouse
                        GROUP BY w_supplierkey
                    ) w ON w_supplierkey = s_suppkey
                    WHERE sum_warehouses IS NULL
                ")) > 0) {
                    return "The supplier with the smallest number of warehouses is " + SQLHandler.ExecuteSQLiteQuery (@"
                        SELECT s_name
                        FROM supplier
                        LEFT JOIN (
                            SELECT w_supplierkey, COUNT(*) sum_warehouses
                            FROM warehouse
                            GROUP BY w_supplierkey
                        ) w ON w_supplierkey = s_suppkey
                        WHERE sum_warehouses IS NULL
                    ");
                }
                return "The supplier with the smallest number of warehouses is " + SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT s_name
                    FROM supplier
                    LEFT JOIN (
                        SELECT w_supplierkey, COUNT(*) sum_warehouses
                        FROM warehouse
                        GROUP BY w_supplierkey
                    ) w ON w_supplierkey = s_suppkey
                    WHERE sum_warehouses = (
                        SELECT MIN(sum_warehouses_tot)
                        FROM (
                            SELECT COUNT(*) sum_warehouses_tot
                            FROM warehouse
                            GROUP BY w_supplierkey
                        )
                    )
                ");
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Lab7MaxWarehouseCapacity")] /* API Endpoints: /api/Lab7MaxWarehouseCapacity */
        public static async Task<string> Lab7MaxWarehouseCapacity ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = "Lab7MaxWarehouseCapacity")] HttpRequest req) {
            try {
                return "The maximum capacity of any supplier's warehouses is " + SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT MAX(sum_cap)
                    FROM (
                        SELECT SUM(w_capacity) sum_cap
                        FROM warehouse
                        GROUP BY w_supplierkey
                    )");
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Lab7EuropeanWarehousesSmallerThanX")] /* API Endpoints: /api/Lab7EuropeanWarehousesSmallerThanX */
        public static async Task<string> Lab7EuropeanWarehousesSmallerThanX ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = "Lab7EuropeanWarehousesSmallerThanX")] HttpRequest req) {
            try {
                string x = (string) req.Query["x"];
                return "Warehouses in Europe with capacity smaller than " + x + " include " + SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT w_name
                    FROM warehouse
                    WHERE w_nationkey IN (
                        SELECT n_nationkey
                        FROM nation
                        WHERE n_regionkey = (
                            SELECT r_regionkey
                            FROM region   
                            WHERE r_name = 'EUROPE'
                        )
                    ) AND w_capacity < " + x);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Lab7WarehouseLargeEnoughForSupplier")] /* API Endpoints: /api/Lab7WarehouseLargeEnoughForSupplier */
        public static async Task<string> Lab7WarehouseLargeEnoughForSupplier ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = "Lab7WarehouseLargeEnoughForSupplier")] HttpRequest req) {
            try {
                string name = (string) req.Query["name"];
                int parts_num = int.Parse (SQLHandler.ExecuteSQLiteQuery (@"
                        SELECT COUNT(*)
                        FROM partsupp
                        WHERE ps_suppkey = (
                            SELECT s_suppkey
                            FROM supplier
                            WHERE s_name = '" + name + @"'
                        )
                    "));
                int cap_num = int.Parse (SQLHandler.ExecuteSQLiteQuery (@"
                        SELECT COUNT(*)
                        FROM partsupp
                        WHERE ps_suppkey = (
                            SELECT s_suppkey
                            FROM supplier
                            WHERE s_name = '" + name + @"'
                        )
                    "));
                if (parts_num < cap_num) {
                    return name + " has enough warehouse space";
                }
                return name + " doesn't have enough warehouse space";
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Lab7WarehousesInNation")] /* API Endpoints: /api/Lab7WarehousesInNation */
        public static async Task<string> Lab7WarehousesInNation ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = "Lab7WarehousesInNation")] HttpRequest req) {
            try {
                string name = (string) req.Query["name"];
                return name + " has warehouses: " + SQLHandler.ExecuteSQLiteQuery (@" 
                    SELECT w_name
                    FROM warehouse
                    WHERE w_nationkey = (
                        SELECT n_nationkey
                        FROM nation
                        WHERE n_name = '" + name + @"'
                    )
                    ORDER BY w_capacity DESC
                ");
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        [FunctionName ("Lab7WarehouseChange")] /* API Endpoints: /api/Lab7WarehouseChange */
        public static async Task<string> Lab7WarehouseChange ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = "Lab7WarehouseChange")] HttpRequest req) {
            try {
                string supp_old = (string) req.Query["supp_old"];
                string supp_old_key = SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT s_suppkey
                    FROM supplier
                    WHERE s_name = '" + supp_old + "'");
                string supp_new = (string) req.Query["supp_new"];
                string supp_new_key = SQLHandler.ExecuteSQLiteQuery (@"
                    SELECT s_suppkey
                    FROM supplier
                    WHERE s_name = '" + supp_new + "'");
                return supp_old + " replaced by " + supp_new + ": " + SQLHandler.ExecuteSQLiteNonQuery (@"
                    UPDATE warehouse
                    SET w_supplierkey = " + supp_new_key + @"
                    WHERE w_supplierkey = " + supp_old_key);
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString ();
        }

        /* Endpoint Functions */
        /* * * * * * * * * * */
        [FunctionName (HTTP.Endpoints.SET)] /* API Endpoints: /api/set?flag=reset, /api/set?flag=add&table=players */
        public static async Task<string> Set ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = HTTP.Endpoints.SET)] HttpRequest req) {
            try {
                /* Reads data into table and returns transaction receipt */
                switch (req.Query[HTTP.Endpoints.Parameters.FLAG]) {
                    case HTTP.Endpoints.Parameters.Values.RESET:

                        /* Pulls galaxy JSON from HTTP Body */
                        dynamic galaxy = await GetBody (req.Body);

                        /* Initializing Table Values */
                        Dictionary<string, List<string>> values = new Dictionary<string, List<string>> { { Database.Tables.Galaxies.ALIAS, new List<string> () },
                            { Database.Tables.Systems.ALIAS, new List<string> () },
                            { Database.Tables.SystemConnections.ALIAS, new List<string> () },
                            { Database.Tables.Planets.ALIAS, new List<string> () },
                            { Database.Tables.Asteroids.ALIAS, new List<string> () }
                        };

                        /* Agregrates Table Values */
                        values[Database.Tables.Galaxies.ALIAS].Add (WrapValues (new string[] {
                            galaxy.id, galaxy.seed
                        }));
                        foreach (dynamic system in galaxy.systems) {
                            values[Database.Tables.Systems.ALIAS].Add (WrapValues (new string[] {
                                system.id, galaxy.id, system.seed, system.position_x, system.position_y
                            }));
                            values[Database.Tables.SystemConnections.ALIAS].AddRange (((IEnumerable<dynamic>) system.connected_systems).Select (
                                connected_system => WrapValues (new string[] {
                                    system.id, connected_system, "1"
                                })
                            ));
                            values[Database.Tables.Planets.ALIAS].AddRange (((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    planet.id, system.id, planet.seed, planet.radius, planet.offset, planet.size, planet.density, planet.composition, planet.is_habitable, planet.is_inhabited, planet.kardashev_level, planet.economy_type
                                })
                            ));
                            values[Database.Tables.Asteroids.ALIAS].AddRange (((IEnumerable<dynamic>) system.asteroids).Select (
                                asteroid => WrapValues (new string[] {
                                    asteroid.id, system.id, asteroid.seed, asteroid.radius, asteroid.offset, asteroid.size, asteroid.density, asteroid.composition, asteroid.is_mineable, asteroid.is_regenerating
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

        [FunctionName (HTTP.Endpoints.GET)] /* API Endpoint: /api/get?table=players&fields=* */
        public static async Task<string> Get ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = HTTP.Endpoints.GET)] HttpRequest req) {
            try {
                /* Returns formatted result of selection query */

                string type = req.Query[HTTP.Endpoints.Parameters.TYPE];
                string id = req.Query[HTTP.Endpoints.Parameters.ID];

                switch (type) {
                    case Database.Tables.Galaxies.ALIAS:

                        // string galaxy_serialized = 

                        SQLHandler.Select (new Dictionary<string, string> { { SQL.COLUMNS, SQL.ALL },
                            { SQL.TABLE, Database.Tables.Galaxies.ALIAS },
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

        [FunctionName (HTTP.Endpoints.UPDATE)] /* API Endpoint: /api/update?table=players */
        public static async Task<string> Update ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.PUT, Route = HTTP.Endpoints.UPDATE)] HttpRequest req) {

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

        [FunctionName (HTTP.Endpoints.RESET)] /* API Endpoint: /api/reset */
        public static async Task<string> Reset ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = HTTP.Endpoints.RESET)] HttpRequest req) {
            try {
                /* Drops old tables and creates new ones with updated fields */
                return SQLHandler.Drop (Database.ALL_TABLES) + SQLHandler.Create (Database.ALL_TABLE_DEFINITIONS);

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