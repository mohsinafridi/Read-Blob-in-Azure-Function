//using System.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace ReadBlobFile
{
    public static class Function1
    {
        [FunctionName("FileOperations")]
        public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)

        //public static void Run([BlobTrigger("hisofiles/{name}.csv", Connection = "AzureWebJobsStorage")] Stream myBlob,
        //    string name, [Table("Employee", Connection = "AzureWebJobsStorage")] IAsyncCollector<Employee> employeeTable,
        //    ILogger log)
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
                CloudBlockBlob blob = container.GetBlockBlobReference("mohsin.csv");
                
                using (var memoryStream = new MemoryStream())
                {
                    blob.DownloadToStreamAsync(memoryStream).GetAwaiter().GetResult();
                    memoryStream.Position = 0;
                    using (var reader = new StreamReader(memoryStream))
                    using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
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
