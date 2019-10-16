using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BitNaughts
{
    public static class FunctionApp
    {
        [FunctionName("GetPlayers")]
        public static async Task<string> GetPlayers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetPlayers")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Running Racing Route");
            
            string output = "STRING: " + System.Environment.GetEnvironmentVariable("Connection String").ToString();

            string query =  "SELECT alias" +
                            "FROM " + req.Query["db"];

            output += "\n" + query + "\n";

            SqlConnection connection = new SqlConnection(System.Environment.GetEnvironmentVariable("Connection String"));
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                using (connection)
                {
                    output = "Connection Established";
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    Object[] values = new Object[reader.FieldCount];
                    int fieldCount = reader.GetValues(values);

                    output += "reader.GetValues retrieved " + fieldCount + "columns.";
                    for (int i = 0; i < fieldCount; i++)
                        output += values[i];
                    
                    reader.Close();
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                return ex.ToString() + output;
            }

            return output;
        }
    }
}
