using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Nancy.Json;

namespace BitNaughts {
    public static class EndpointHandler {

        public const string DELIMITER = ",";
        public const string NEW_LINE = "\n";

        /* Assembles HTTP Body byte-stream into JSON */
        public static async Task<dynamic> GetBody (HttpRequest req) {
            string requestBody = await new StreamReader (req.Body).ReadToEndAsync ();
            return Newtonsoft.Json.JsonConvert.DeserializeObject (requestBody);
        }
        
        [FunctionName ("Create")] /* API Endpoint: /api/create?table=players */
        public static async Task<string> Create ([HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "create")] HttpRequest req, ILogger log) {

            /* Reads data into table and returns transaction receipt */
            dynamic req_body = await GetBody (req);
            return QueryHandler.ExecuteNonQuery (
                String.Format (
                    "INSERT INTO {0} VALUES ({1})", /* SQL Query to be executed */
                    req.Query["table"],
                    String.Join (
                        DELIMITER,
                        req_body.values.ToObject<string[]> ()
                    )
                )
            );
        }

        [FunctionName ("Read")] /* API Endpoint: /api/read?table=players&fields=* */
        public static async Task<string> Read ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "read")] HttpRequest req, ILogger log) {

            /* Returns CSV formatted result of selection query */
            return QueryHandler.ExecuteQuery (
                String.Format (
                    "SELECT {1} FROM {0}", /* SQL Query to be executed */
                    req.Query["table"],
                    req.Query["fields"]
                )
            );
        }

        [FunctionName ("Update")] /* API Endpoint: /api/update?table=players */
        public static async Task<string> Update ([HttpTrigger (AuthorizationLevel.Anonymous, "put", Route = "update")] HttpRequest req, ILogger log) {

            /* Overrides data in table and returns transaction receipt */
            dynamic req_body = await GetBody (req);
            return QueryHandler.ExecuteNonQuery (
                String.Format (
                    "UPDATE {0} SET {1} WHERE {2}", /* SQL Query to be executed */
                    req.Query["table"],
                    String.Join (
                        DELIMITER,
                        req_body.values.ToObject<string[]> ()
                    ),
                    req_body.condition
                )
            );
        }

        [FunctionName ("Delete")] /* API Endpoint: /api/delete?table=players */
        public static async Task<string> Delete ([HttpTrigger (AuthorizationLevel.Anonymous, "delete", Route = "delete")] HttpRequest req, ILogger log) {

            /* Removes data in table and returns transaction receipt */
            dynamic req_body = await GetBody (req);
            return QueryHandler.ExecuteQuery (
                String.Format (
                    "DELETE FROM {0} WHERE {1}", /* SQL Query to be executed */
                    req.Query["table"],
                    req_body.condition
                )
            );
        }
    }
}