// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Azure.Messaging.EventGrid;
using ChoETL;
using EventToADLSParquet.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace EventToADLSParquet
{
    public static class EventSubscriber
    {
        [FunctionName("EventSubscriber")]
        public static void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent, 
            ILogger log)
        {
            log.LogInformation("C# Event Grid trigger function processed a request.");
            log.LogInformation($"Received events: {eventGridEvent.Data}");

            // Handle the custom event
            var contact = eventGridEvent.Data.ToObjectFromJson<Contact>();
            using (var writer = new ChoParquetWriter("output.parquet"))
            {
                writer.Write(contact);
            }
        }
    }
}