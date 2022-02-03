// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Azure.Messaging.EventGrid;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using ChoETL;
using EventToADLSParquet.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace EventToADLSParquet
{
    public class EventSubscriber
    {
        [FunctionName("EventSubscriber")]
        public static void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent, 
            ILogger log)
        {
            log.LogInformation("C# Event Grid trigger function processed a request.");
            log.LogInformation($"Received events: {eventGridEvent.Data}");

            // Handle the custom event as collection for Parquet support
            var records = new List<Contact>
            {
                eventGridEvent.Data.ToObjectFromJson<Contact>()
            };

            // Write converted object to Parquet and upload to storage
            StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(Environment.GetEnvironmentVariable("Storage:AccountName"), Environment.GetEnvironmentVariable("Storage:AccountKey"));

            // Create DataLakeServiceClient using StorageSharedKeyCredentials
            var serviceUri = new Uri(Environment.GetEnvironmentVariable("Storage:ServiceUri"));
            DataLakeServiceClient serviceClient = new DataLakeServiceClient(serviceUri, sharedKeyCredential);

            // Get a reference to a filesystem named after event subject and then create it
            DataLakeFileSystemClient filesystem = serviceClient.GetFileSystemClient("events");
            filesystem.CreateIfNotExists();

            var eventDate = eventGridEvent.EventTime.Date.ToString("yyyy-MM-dd");
            DataLakeDirectoryClient directory = filesystem.GetDirectoryClient($"{eventGridEvent.Subject}/{eventDate}");
            directory.CreateIfNotExists();

            // Create a DataLake File using a DataLake Directory
            DataLakeFileClient file = directory.GetFileClient($"{eventGridEvent.Subject}_{eventGridEvent.Id}.parquet");

            // Write data
            using var outStream = file.OpenWrite(true);
            using ChoParquetWriter writer = new(outStream);
            writer.Write(records);
        }
    }
}