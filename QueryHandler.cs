using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

public static class QueryHandler {

    public const string DELIMITER = ",";
    public const string NEW_LINE = "\n";
    public const string ERROR_MESSAGE = "ERROR";

    /* Helper Function for managing database connection, running commands, and returning results */
    public static string[] ExecuteQuery (string query) {
        try {
            /* Defines connection parameters and query logic */
            SqlConnection connection = new SqlConnection (System.Environment.GetEnvironmentVariable ("Connection String"));
            SqlCommand command = new SqlCommand (query, connection);

            /* Connects to database and executes query */
            connection.Open ();
            SqlDataReader reader = command.ExecuteReader ();

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

            /* Returns array of strings, one string per row returned from query */
            return results.ToArray ();

        } catch (Exception ex) {
            return new string[] {
                ERROR_MESSAGE,
                ex.ToString (),
                query
            };
        }
    }

    /* Helper Function for managing database connection, running commands, and returning results */
    public static string[] ExecuteNonQuery (string query) {
        try {
            /* Defines connection parameters and query logic */
            SqlConnection connection = new SqlConnection (System.Environment.GetEnvironmentVariable ("Connection String"));
            SqlCommand command = new SqlCommand (query, connection);

            /* Connects to database and executes non-query */
            connection.Open ();
            int rows_modified = command.ExecuteNonQuery ();

            /* Returns number of rows modified */
            return new string[] {
                String.Format (
                    "{0} Rows Modified",
                    rows_modified
                )
            };

        } catch (Exception ex) {
            return new string[] {
                ERROR_MESSAGE,
                ex.ToString (),
                query
            };
        }
    }
}