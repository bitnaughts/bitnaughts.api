using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ModestoMoves
{
    public static class FunctionApp
    {
        [FunctionName("GetPlayers")]
        public static async Task<JObject> GetPlayers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetPlayers")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Running Racing Route");
            
            string output = "";

            // string raceID = req.Query["raceID"];

            SqlConnection connectionString = new SqlConnection(System.Environment.GetEnvironmentVariable("Connection String"));
            SqlCommand querystring = new SqlCommand("SELECT alias FROM bitnaughts-db.Players", connectionString);


            try
            {
                using (connectionString)
                {
                    output = "Connection Established";
                    connectionString.Open();

                    SqlDataReader reader = querystring.ExecuteReader();

                    Object[] values = new Object[reader.FieldCount];
                    int fieldCount = reader.GetValues(values);

                    output += "reader.GetValues retrieved " + fieldCount + "columns.";
                    for (int i = 0; i < fieldCount; i++)
                        output += values[i];
                    
                    reader.Close();
                    connectionString.Close();

                }
            }
            catch (Exception ex)
            {
                return new JObject().Add("Error", ex);
            }

            return new JObject().Add("Output", output);
        }
    }
}
