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

public static class SQLHandler {

    public static string Select (Dictionary<string, string> parameters) {
        if (parameters.ContainsKey (SQL.CONDITION)) {
            return ExecuteQuery (String.Format (
                "SELECT {0} FROM {1} WHERE {2}", /* SQL Query to be executed */
                parameters[SQL.COLUMNS],
                parameters[SQL.TABLE],
                parameters[SQL.CONDITION]
            ));
        }
        else {
            return ExecuteQuery (String.Format (
                "SELECT {0} FROM {1}", /* SQL Query to be executed */
                parameters[SQL.COLUMNS],
                parameters[SQL.TABLE]
            ));
        }
    }

    public static string Update (Dictionary<string, string> parameters) {
        string receipt = String.Format ("{0}: Updating Table({1})\n",
            GetRecepitDate (),
            parameters[SQL.TABLE]
        );
        receipt += ExecuteNonQuery (String.Format (
            "UPDATE {0} SET {1} WHERE {2}", /* SQL Query to be executed */
            parameters[SQL.TABLE],
            SQL.IsEqual (parameters[SQL.COLUMN], parameters[SQL.VALUE]),
            parameters[SQL.CONDITION]
        ));
        return receipt;
    }
    public static string Delete (Dictionary<string, string> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into Tables({2})\n",
            GetRecepitDate (),
            values.Count,
            String.Join (SQL.Format.DELIMITER, new List<string> (values.Keys).ToArray ())
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
    public static string Drop (string[] tables) {
        string receipt = String.Format ("{0}: Dropping {1} tables\n",
            GetRecepitDate (),
            tables.Length
        );
        foreach (string table in tables) {
            receipt += ExecuteNonQuery (String.Format (
                "DROP TABLE {0}", /* SQL Query to be executed when a condition is specified */
                table
            ));
        }
        return receipt;
    }
    public static string Create (string[] tables_data) {
        string receipt = String.Format ("{0}: Creating {1} tables\n",
            GetRecepitDate (),
            tables_data.Length
        );
        foreach (string table_data in tables_data) {
            receipt += ExecuteNonQuery (
                table_data
            );
        }
        return receipt;
    }
    public static string Insert (Dictionary<string, List<string>> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into Tables({2})\n",
            GetRecepitDate (),
            values.Count,
            String.Join (SQL.Format.DELIMITER, new List<string> (values.Keys).ToArray ())
        );
        foreach (KeyValuePair<string, List<string>> value in values) {
            receipt += Insert (value.Key, value.Value);
        }
        return receipt;
    }
    public static string Insert (string table, List<string> values) {
        string receipt = String.Format ("{0}: Adding {1} rows into {2}\n",
            GetRecepitDate (),
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
                String.Join (SQL.Format.DELIMITER, values.Skip (batch_index).Take (SQL.INSERT_BATCH_SIZE).ToArray ())
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
                                    SQL.Format.DELIMITER,
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
                            SQL.Format.NEW_LINE,
                            results.ToArray ()
                        );
                    }
                }
            }
        } catch (Exception ex) {
            return String.Format (
                "Error({0}): {1}\n",
                Receiptize (query),
                Receiptize (ex.ToString ())
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
                        Receiptize (query),
                        rows_modified
                    );
                }
            }
        } catch (Exception ex) {
            return String.Format (
                "Error({0}): {1}\n",
                Receiptize (query),
                Receiptize (ex.ToString ())
            );
        }
    }

    /* Throw-away SQLite implementation */
    public static string ExecuteSQLiteQuery (string query) {
        try {
            /* Defines connection parameters and query logic */
            using (SQLiteConnection conn = new SQLiteConnection (@"Data Source=C:\Users\Mutilar\Desktop\TPCH.db;Version=3; FailIfMissing=True; Foreign Keys=True;")) {
                using (SQLiteCommand cmd = new SQLiteCommand (query, conn)) {

                    /* Connects to database and executes query */
                    conn.Open ();
                    using (SQLiteDataReader reader = cmd.ExecuteReader ()) {

                        /* Holds row results as they are read */
                        List<string> results = new List<string> ();
                        while (reader.Read ()) {

                            /* Dumps values into Object array */
                            Object[] fields = new Object[reader.FieldCount];
                            reader.GetValues (fields);

                            /* Adds row result as delimiter-seperated values */
                            results.Add (
                                String.Join (
                                    SQL.Format.DELIMITER,
                                    fields.Where (x => x != null)
                                    .Select (x => x.ToString ())
                                    .ToArray ()
                                )
                            );
                        }
                        /* Closes the database connection */
                        conn.Close ();

                        /* Returns formatted results from query */
                        return String.Join (
                            SQL.Format.NEW_LINE,
                            results.ToArray ()
                        );
                    }
                }
            }
        } catch (Exception ex) {
            return String.Format (
                "Error({0}): {1}\n",
                Receiptize (query),
                Receiptize (ex.ToString ())
            );
        }
    }
    public static string ExecuteSQLiteNonQuery (string[] queries) {
        string result = "";
        foreach (string query in queries) {
            result += ExecuteSQLiteNonQuery (query);
        }
        return result;
    }
    public static string ExecuteSQLiteNonQuery (string query) {
        try {
            /* Defines connection parameters and query logic */
            using (SQLiteConnection conn = new SQLiteConnection (@"Data Source=C:\Users\Mutilar\Desktop\TPCH.db;Version=3; FailIfMissing=True; Foreign Keys=True;")) {
                using (SQLiteCommand cmd = new SQLiteCommand (query, conn)) {

                    /* Connects to database and executes query */
                    conn.Open ();
                    int rows_modified = cmd.ExecuteNonQuery ();

                    /* Returns number of rows modified */
                    return String.Format (
                        "Query({0}): {1} Row(s) Modified\n",
                        Receiptize (query),
                        rows_modified
                    );
                }
            }
        } catch (Exception ex) {
            return String.Format (
                "Error({0}): {1}\n",
                Receiptize (query),
                Receiptize (ex.ToString ())
            );
        }
    }
    /* Scrubs and truncates large texts */
    public static string Receiptize (string text) {
        string output = "";
        foreach (char curr_char in text) {
            if (SQL.Format.MAX_CHARS_RETURED > 0 && output.Length == SQL.Format.MAX_CHARS_RETURED) return output + " ..."; /* Immediately returns when output is too long */
            else if (SQL.Format.VOIDED_CHARS.Contains (curr_char)) continue; /* Skips character if in illegal set */
            else if (output != "" && curr_char == ' ' && output.Last () == ' ') continue; /* Skips whitespace longer than length one */
            else output += curr_char; /* Appends valid character from original string */
        }
        return output;
    }
    public static string GetRecepitDate () {
        return "3" + DateTime.Now.ToString ("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK").Substring (1);
    }
}