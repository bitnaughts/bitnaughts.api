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



    public static string Select (Dictionary<string, string> parameters) {
        return ExecuteQuery (String.Format (
            "SELECT {1} FROM {0} WHERE {2}", /* SQL Query to be executed */
            parameters[SQL.TABLE],
            parameters[SQL.COLUMNS],
            parameters[SQL.CONDITION]
        ));
    }

    public static string Delete (Dictionary<string, string> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into Tables({2})\n",
            DateTime.Now.ToShortTimeString (),
            values.Count,
            String.Join (DELIMITER, new List<string> (values.Keys).ToArray ())
        );
        foreach (KeyValuePair<string, string> value in values) {
            receipt += (value.Value == SQL.ALL) ?
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

    public static string Drop(List<string> tables) {
        string receipt = String.Format ("{0}: Dropping {1} tables\n",
            DateTime.Now.ToShortTimeString (),
            tables.Count
        );
        foreach (string table in tables) {
            receipt += ExecuteNonQuery(String.Format(
                "DROP TABLE {0}",
                table
            ));
        }
        return receipt;
    }    
    public static string Create(List<string> tables_data) {
        string receipt = String.Format ("{0}: Creating {1} tables\n",
            DateTime.Now.ToShortTimeString (),
            tables_data.Count
        );
        foreach (string table_data in tables_data) {
            receipt += ExecuteNonQuery(
                table_data
            );
        }
        return receipt;
    }


    public static string Insert (Dictionary<string, List<string>> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into Tables({2})\n",
            DateTime.Now.ToShortTimeString (),
            values.Count,
            String.Join (DELIMITER, new List<string> (values.Keys).ToArray ())
        );
        foreach (KeyValuePair<string, List<string>> value in values) {
            receipt += Insert (value.Key, value.Value);
        }
        return receipt;
    }

    public static string Insert (string table, List<string> values) {
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
                String.Join (DELIMITER, values.Skip (batch_index).Take (SQL.INSERT_BATCH_SIZE).ToArray ())
            ));

            /* Keeping track of how many values have been added */
            batch_index += SQL.INSERT_BATCH_SIZE;
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

    public static IEnumerable<string> GetSQLTableDefinitions(string folder_path) {
        return Directory.EnumerateFiles (folder_path, FileFormat.SQL_FILES);
    }
}