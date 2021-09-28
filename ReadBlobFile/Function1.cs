//using System.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;

namespace ReadBlobFile
{
    public static class Function1
    {
        [FunctionName("FileOperations")]
        // public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)

        public static void Run([BlobTrigger("hisofiles/{name}", Connection = "")] Stream myBlob, string name, ILogger log)
        {
            
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                var ConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                // Setup the connection to the storage account
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
                // Connect to the blob storage
                CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
                // Connect to the blob container
                CloudBlobContainer container = serviceClient.GetContainerReference("hisofiles");
                // Connect to the blob file
                CloudBlockBlob blob = container.GetBlockBlobReference(name);

                using (var memoryStream = new MemoryStream())
                {
                    blob.DownloadToStreamAsync(memoryStream).GetAwaiter().GetResult();
                  //  blob.UploadFromStreamAsync(memoryStream);  // upload blob to storage.
                    memoryStream.Position = 0;
                    using (var reader = new StreamReader(memoryStream))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<Foo>();
                        foreach (Foo item in records)
                        {
                            Console.WriteLine(item.Name);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        //private async static Task CreateBlob(string name, string data)
        //{
        //    CloudBlobClient client;
        //    CloudBlobContainer container;
        //    CloudBlockBlob blob;

        //    var cs = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cs);
        //    client = storageAccount.CreateCloudBlobClient();

        //    container = client.GetContainerReference("hisofiles");

        //    await container.CreateIfNotExistsAsync();

        //    blob = container.GetBlockBlobReference(name);
        //    blob.Properties.ContentType = "application/json";

        //    using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
        //    {
        //        await blob.UploadFromStreamAsync(stream);
        //    }
        //}
    }

    public class Foo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
    //public class Employee : TableEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public int Age { get; set; }
    //}
}
