using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.ComponentModel;

namespace MenuFunction
{


    public static class RetrieveOrdersFunction
    {


        [FunctionName("RetrieveOrdersFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string connString = Environment.GetEnvironmentVariable("ConnectionString");

            
            const string BLOB_CONTAINER = "orders";


            BlobServiceClient blobServiceClient = new BlobServiceClient(connString);
            
            // Get the container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("orders");
            var RawArray="";
            // List all blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
                string BlobUri = blobItem.Name;
                var blobClient = new BlobClient(connString, BLOB_CONTAINER, BlobUri);                                                        // check file download was success or no
                var content = await blobClient.OpenReadAsync(); // I don't know what you want to do with this
                StreamReader reader = new StreamReader(content);
                string RawMenu = reader.ReadToEnd();
                RawArray += RawMenu+ ",";
            }

            var JsonToReturn = RawArray.Substring(0,(RawArray.Length-1));



            return new OkObjectResult("["+ JsonToReturn + "]");





        }
    }
}
