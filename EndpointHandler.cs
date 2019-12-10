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

        /* Endpoint Functions */
        /* * * * * * * * * * */
        [FunctionName (HTTP.Endpoints.SET)] /* API Endpoints: /api/set?flag=reset, /api/set?flag=add&table=players */
        public static async Task<string> Set ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = HTTP.Endpoints.SET)] HttpRequest req) {
            try {
                /* Reads data into table and returns transaction receipt */
                string flag = req.Query[HTTP.Endpoints.Parameters.FLAG];
                string table = req.Query[HTTP.Endpoints.Parameters.TABLE];

                switch (flag) {
                    case HTTP.Endpoints.Parameters.Values.ADD:
                        switch (table) {
                            case Ships.ALIAS:
                                dynamic ship = await GetBody (req.Body);
                                return SQLHandler.Insert (
                                    Ships.ALIAS,
                                    new List<string> () {
                                        WrapValues (new string[] {
                                            ship.id, ship.player_id, ship.current_system, ship.name, ship.data, ship.position_x, ship.position_y
                                        })
                                    }
                                );
                        }
                        break;
                    case HTTP.Endpoints.Parameters.Values.RESET:

                        /* Pulls galaxy JSON from HTTP Body */
                        dynamic galaxy = await GetBody (req.Body);

                        /* Initializing Table Values */
                        Dictionary<string, List<string>> values = new Dictionary<string, List<string>> { { Galaxies.ALIAS, new List<string> () },
                            { Systems.ALIAS, new List<string> () },
                            { SystemConnections.ALIAS, new List<string> () },
                            { Planets.ALIAS, new List<string> () },
                            { Asteroids.ALIAS, new List<string> () }
                        };

                        /* Agregrates Table Values */
                        values[Galaxies.ALIAS].Add (WrapValues (new string[] {
                            galaxy.id, galaxy.seed
                        }));
                        foreach (dynamic system in galaxy.systems) {
                            values[Systems.ALIAS].Add (WrapValues (new string[] {
                                system.id, galaxy.id, system.seed, system.position_x, system.position_y
                            }));
                            values[SystemConnections.ALIAS].AddRange (((IEnumerable<dynamic>) system.connected_systems).Select (
                                connected_system => WrapValues (new string[] {
                                    system.id, connected_system, "1"
                                })
                            ));
                            values[Planets.ALIAS].AddRange (((IEnumerable<dynamic>) system.planets).Select (
                                planet => WrapValues (new string[] {
                                    planet.id, system.id, planet.seed, planet.radius, planet.theta, planet.size, planet.density, planet.composition, planet.is_habitable, planet.is_inhabited, planet.kardashev_level, planet.economy_type
                                })
                            ));
                            values[Asteroids.ALIAS].AddRange (((IEnumerable<dynamic>) system.asteroids).Select (
                                asteroid => WrapValues (new string[] {
                                    asteroid.id, system.id, asteroid.seed, asteroid.radius, asteroid.theta, asteroid.size, asteroid.density, asteroid.composition, asteroid.is_mineable, asteroid.is_regenerating
                                })
                            ));
                        }
                        return SQLHandler.Delete (values.ToDictionary (value => value.Key, value => SQL.ALL)) + /* For every table referenced, clear all existing values */
                            SQLHandler.Insert (values); /* For every table referenced, inject values */
                }
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString (); /* InvalidOperationException returned if attempting to add a value not supported yet */
        }

        [FunctionName (HTTP.Endpoints.LOGIN)] /* API Endpoints: /api/login?player=1 */
        public static async Task<string> Login ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = HTTP.Endpoints.LOGIN)] HttpRequest req) {
            try {
                string player = req.Query[HTTP.Endpoints.Parameters.PLAYER];

                string session_id = SQLHandler.Select (new Dictionary<string, string> { { SQL.TABLE, SessionHistory.ALIAS },
                    { SQL.COLUMNS, SQL.COUNT }
                });

                return SQLHandler.Update (new Dictionary<string, string> { { SQL.TABLE, Players.ALIAS },
                    { SQL.CONDITION, SQL.IsEqual (Players.ID, player) },
                    { SQL.COLUMN, Players.CURRENT_SESSION },
                    { SQL.VALUE, session_id }
                }) + SQLHandler.Insert (
                    SessionHistory.ALIAS,
                    new List<string> () {
                        WrapValues (new string[] {
                            session_id,
                            player,
                            DateTime.UtcNow.ToString (SQL.Format.DATETIME),
                            "NULL"
                        })
                    }
                );
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString (); /* InvalidOperationException returned if attempting to mine more than asteroid has */
        }

        [FunctionName (HTTP.Endpoints.LOGOUT)] /* API Endpoints: /api/logout?player=1 */
        public static async Task<string> Logout ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = HTTP.Endpoints.LOGOUT)] HttpRequest req) {
            try {
                string player = req.Query[HTTP.Endpoints.Parameters.PLAYER];

                string session_id = SQLHandler.Select (new Dictionary<string, string> { { SQL.TABLE, SessionHistory.ALIAS },
                    { SQL.COLUMNS, Players.CURRENT_SESSION },
                    { SQL.CONDITION, SQL.IsEqual (Players.ID, player) }
                });

                return SQLHandler.Update (new Dictionary<string, string> { { SQL.TABLE, Players.ALIAS },
                    { SQL.CONDITION, SQL.IsEqual (Players.ID, player) },
                    { SQL.COLUMN, Players.CURRENT_SESSION },
                    { SQL.VALUE, "-1" }
                }) + SQLHandler.Update (new Dictionary<string, string> { { SQL.TABLE, SessionHistory.ALIAS },
                    { SQL.CONDITION, SQL.IsEqual (SessionHistory.ID, session_id) },
                    { SQL.COLUMN, SessionHistory.LOG_OUT_DATE },
                    { SQL.VALUE, DateTime.UtcNow.ToString (SQL.Format.DATETIME) }
                });
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString (); /* InvalidOperationException returned if attempting to mine more than asteroid has */
        }

        [FunctionName (HTTP.Endpoints.FIGHT)] /* API Endpoints: /api/fight?ship_1=0&ship_2=1 */
        public static async Task<string> Fight ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = HTTP.Endpoints.FIGHT)] HttpRequest req) {
            try {
                string ship_aggressor = req.Query[HTTP.Endpoints.Parameters.SHIP_1];
                string ship_defender = req.Query[HTTP.Endpoints.Parameters.SHIP_2];

                string combat_id = SQLHandler.Select (new Dictionary<string, string> { { SQL.TABLE, CombatHistory.ALIAS },
                    { SQL.COLUMNS, SQL.COUNT }
                });

                return SQLHandler.Insert (
                    CombatHistory.ALIAS,
                    new List<string> () {
                        WrapValues (new string[] {
                            combat_id,
                            ship_aggressor,
                            ship_defender,
                            DateTime.UtcNow.ToString (SQL.Format.DATETIME)
                        })
                    }
                );
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString (); /* InvalidOperationException returned if attempting to mine more than asteroid has */
        }

        [FunctionName (HTTP.Endpoints.VISIT)] /* API Endpoints: /api/visit?planet=12&ship=5 */
        public static async Task<string> Visit ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = HTTP.Endpoints.VISIT)] HttpRequest req) {
            try {
                string ship = req.Query[HTTP.Endpoints.Parameters.SHIP];
                string planet = req.Query[HTTP.Endpoints.Parameters.PLANET];

                return SQLHandler.Insert (
                    Visits.ALIAS,
                    new List<string> () {
                        WrapValues (new string[] {
                            ship,
                            planet,
                            DateTime.UtcNow.ToString (SQL.Format.DATETIME)
                        })
                    }
                );
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString (); /* InvalidOperationException returned if attempting to mine more than asteroid has */
        }

        [FunctionName (HTTP.Endpoints.MINE)] /* API Endpoints: /api/mine?asteroid=12&ship=5&amount=44 */
        public static async Task<string> Mine ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.POST, Route = HTTP.Endpoints.MINE)] HttpRequest req) {
            try {
                string ship = req.Query[HTTP.Endpoints.Parameters.SHIP];
                string asteroid = req.Query[HTTP.Endpoints.Parameters.ASTEROID];
                double mined_amount = double.Parse (req.Query[HTTP.Endpoints.Parameters.AMOUNT]);

                /* Gets current asteroid size to determine if mining fully depleted asteroid */
                double asteroid_size = 0;
                if (double.TryParse (SQLHandler.Select (new Dictionary<string, string> { { SQL.TABLE, Asteroids.ALIAS },
                        { SQL.COLUMNS, Asteroids.SIZE },
                        { SQL.CONDITION, SQL.IsEqual (Asteroids.ID, asteroid) }
                    }), out asteroid_size)) {
                    /* Asteroid was not fully mined */
                    if (asteroid_size > mined_amount) {
                        /* Reduce size of asteroid and pass to ship */
                        return SQLHandler.Update (new Dictionary<string, string> { { SQL.TABLE, Asteroids.ALIAS },
                            { SQL.CONDITION, SQL.IsEqual (Asteroids.ID, asteroid) },
                            { SQL.COLUMN, Asteroids.SIZE },
                            { SQL.VALUE, (asteroid_size - mined_amount).ToString ("F") }
                        }) + SQLHandler.Update (new Dictionary<string, string> { { SQL.TABLE, Ships.ALIAS },
                            { SQL.CONDITION, SQL.IsEqual (Ships.ID, ship) },
                            { SQL.COLUMN, Ships.DATA },
                            { SQL.VALUE, mined_amount.ToString ("F") }
                        }) + SQLHandler.Insert (
                            Mines.ALIAS,
                            new List<string> () {
                                WrapValues (new string[] {
                                    ship,
                                    asteroid,
                                    mined_amount.ToString ("F"),
                                    DateTime.UtcNow.ToString (SQL.Format.DATETIME)
                                })
                            }
                        );
                        /* Asteroid was fully depleted when mined */
                    } else if (asteroid_size == mined_amount) {
                        /* Delete asteroid, give all size to ship */
                        return SQLHandler.Delete (new Dictionary<string, string> { { Asteroids.ALIAS, SQL.IsEqual (Asteroids.ID, asteroid) } }) +
                            SQLHandler.Update (new Dictionary<string, string> { { SQL.TABLE, Ships.ALIAS },
                                { SQL.CONDITION, SQL.IsEqual (Ships.ID, ship) },
                                { SQL.COLUMN, Ships.DATA },
                                { SQL.VALUE, mined_amount.ToString ("F") }
                            }) + SQLHandler.Insert (
                                Mines.ALIAS,
                                new List<string> () {
                                    WrapValues (new string[] {
                                        ship,
                                        asteroid,
                                        mined_amount.ToString ("F"),
                                        DateTime.UtcNow.ToString (SQL.Format.DATETIME)
                                    })
                                }
                            );
                    }
                } else {
                    return "Asteroid " + asteroid + " does not exist\n";
                }
                return "Not possible?";
            } catch (Exception ex) {
                return ex.ToString ();
            }
            return new InvalidOperationException ().ToString (); /* InvalidOperationException returned if attempting to mine more than asteroid has */
        }

        [FunctionName (HTTP.Endpoints.GET)] /* API Endpoint: /api/get?type=players&id=1, /api/get?type=systems&id=22 */
        public static async Task<string> Get ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.GET, Route = HTTP.Endpoints.GET)] HttpRequest req) {
            try {
                /* Returns formatted result of selection query */

                string type = req.Query[HTTP.Endpoints.Parameters.TYPE];
                string id = req.Query[HTTP.Endpoints.Parameters.ID];

                switch (type) {
                    case Ships.ALIAS:
                        return SQLHandler.Select (new Dictionary<string, string> { { SQL.COLUMNS, SQL.ALL },
                            { SQL.TABLE, Ships.ALIAS },
                            { SQL.CONDITION, Ships.ID + SQL.EQUALS + id }
                        });
                    case Systems.ALIAS:
                        return String.Format (
                            "{{ \"system\": \"{0}\", \"planets\": \"{1}\", \"asteroids\": \"{2}\", \"ships\": \"{3}\" }}",
                            SQLHandler.Select (SQL.ALL, Systems.ALIAS, SQLHandler.Equals(Systems.ID, id)),
                            SQLHandler.Select (SQL.ALL, Planets.ALIAS, SQLHandler.Equals(Planets.SYSTEM_ID, id)),
                            SQLHandler.Select (SQL.ALL, Asteroids.ALIAS, SQLHandler.Equals(Asteroids.SYSTEM_ID, id)),
                            SQLHandler.Select (SQL.ALL, Ships.ALIAS, SQLHandler.Equals(Ships.SYSTEM_ID, id))
                        );
                    case Galaxies.ALIAS:
                        //We only want the:
                        //  - System locations
                        //  - System connections
                        //  - any parameters relevant to rendering the two items above
                        SQLHandler.Select (new Dictionary<string, string> { { SQL.COLUMNS, SQL.ALL },
                            { SQL.TABLE, Galaxies.ALIAS },
                            { SQL.CONDITION, Galaxies.ID + SQL.EQUALS + id }
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

        [FunctionName (HTTP.Endpoints.UPDATE)] /* API Endpoint: /api/update?table=dbo.Ships */
        public static async Task<string> Update ([HttpTrigger (AuthorizationLevel.Anonymous, HTTP.PUT, Route = HTTP.Endpoints.UPDATE)] HttpRequest req) {
            try {
                /* Overrides data in table and returns transaction receipt */
                /* Idempotent... */
                string table = req.Query[HTTP.Endpoints.Parameters.FLAG];
                switch (table) {
                    case Ships.ALIAS:
                        dynamic ship = await GetBody (req.Body);
                        // return SQLHandler.Update (
                        // );
                        break;
                }
            } catch (Exception ex) {
                return ex.ToString ();
            }
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
            return "(" + String.Join (SQL.Format.DELIMITER, values.Select (value => WrapValue (value))) + ")";
        }
        /* Checking if value is non-numeric to add ''s */
        static float value_numeric = 0;
        public static string WrapValue (string value) {
            if (value == "NULL") return value;
            return float.TryParse (value, out value_numeric) ? value : "'" + value + "'";
        }
    }
}