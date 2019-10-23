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

public static class SQLHandler {

    /* Result-formatting constants */
    public const string DELIMITER = ",",
        NEW_LINE = "\n";

    /* Common conditions */
    public const string ALL = "*";

    /* TABLE NAMES */
    public const string GALAXIES = "dbo.Galaxies",
        SYSTEMS = "dbo.Systems",
        SYSTEM_LINKS = "dbo.SystemLinks",
        SYSTEM_CONNECTIONS = "dbo.SystemConnections",
        PLANETS = "dbo.Planets",
        PLANET_LINKS = "dbo.PlanetLinks",
        ASTEROIDS = "dbo.Asteroids",
        ASTEROID_LINKS = "dbo.AsteroidLinks";

    /* MS SQL has been shown to perform best when inserting groups of 25 values at a time. See https://www.red-gate.com/simple-talk/sql/performance/comparing-multiple-rows-insert-vs-single-row-insert-with-three-data-load-methods/ */
    public const int INSERT_BATCH_SIZE = 25;

    /*  */
    public static string DeleteFrom (Dictionary<string, string> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into Tables({2})\n",
            DateTime.Now.ToShortTimeString (),
            values.Count,
            String.Join (DELIMITER, new List<string> (values.Keys).ToArray ())
        );
        foreach (KeyValuePair<string, string> value in values) {
            receipt += (value.Value == ALL) ?
                ExecuteNonQuery (String.Format (
                    "DELETE FROM {0}", /* SQL Query to be executed when no condition is specified*/
                    value.Key
                )) :
                ExecuteNonQuery (String.Format (
                    "DELETE FROM {0} WHERE {1}", /* SQL Query to be executed when a condition is specified */
                    value.Key,
                    value.Value
                ));
        }
        return receipt;
    }

    public static string InsertInto (Dictionary<string, List<string>> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into Tables({2})\n",
            DateTime.Now.ToShortTimeString (),
            values.Count,
            String.Join (DELIMITER, new List<string> (values.Keys).ToArray ())
        );
        foreach (KeyValuePair<string, List<string>> value in values) {
            receipt += InsertInto (value.Key, value.Value);
        }
        return receipt;
    }

    public static string InsertInto (string table, List<string> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into {2}\n",
            DateTime.Now.ToShortTimeString (),
            values.Count,
            table
        );
        /* While there are still too many values to insert at once */
        int batch_index = 0;
        while (values.Count - batch_index > 0) {

            /* Insert a batch of values */
            receipt += ExecuteNonQuery (String.Format (
                "INSERT INTO {0} VALUES {1}", /* SQL Query to be executed */
                table,
                String.Join (DELIMITER, values.Skip (batch_index).Take (INSERT_BATCH_SIZE).ToArray ())
            ));

            /* Keeping track of how many values have been added */
            batch_index += INSERT_BATCH_SIZE;
        }
        return receipt;
    }

    /* Manages database connection, runs queries, and returns results */
    public static string ExecuteQuery (string[] queries) {
        string result = "";
        foreach (string query in queries) {
            result += ExecuteQuery (query);
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
                                ) + "\n"
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
                "Error({0}): {1}\n",
                query.Length > 50 ? query.Substring (0, 50) + "..." : query,
                ex.ToString ()
            );
        }
    }

    /* Manages database connection, runs commands, and returns receipts */
    public static string ExecuteNonQuery (string[] queries) {
        string result = "";
        foreach (string query in queries) {
            result += ExecuteNonQuery (query);
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
                        "Query({0}): {1} Row(s) Modified\n",
                        query.Length > 50 ? query.Substring (0, 50) + "..." : query,
                        rows_modified
                    );
                }
            }
        } catch (Exception ex) {
            return String.Format (
                "Error({0}): {1}\n",
                query.Length > 50 ? query.Substring (0, 50) + "..." : query,
                ex.ToString ()
            );
        }
    }
}