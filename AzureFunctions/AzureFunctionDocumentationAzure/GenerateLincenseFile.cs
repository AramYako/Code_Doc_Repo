using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctionDocumentationAzure
{
    public static class GenerateLincenseFile
    {
        [FunctionName("GenerateLincenseFile")]

        public static async Task Run([QueueTrigger("%GenerateFileQueueTriggerName%", Connection = "AzureWebJobsStorage")]Order order,
            IBinder binder,
            ILogger log)
        {
            //Ibinder good to use when we need a paramter that can only be fetch during runtime
            var outputBlob = await binder.BindAsync<TextWriter>(
                new BlobAttribute($"licenses/{order.OrderId}.json")
                {
                     Connection = "BlobStorageConnectionString"
                }
                );;


            log.LogInformation($"Creating blob order for orderId {order.OrderId}");
            outputBlob.Write("{");
            outputBlob.WriteLine($"\"OrderId\": \"{order.OrderId}\",");
            outputBlob.WriteLine($"\"Email\": \"{order.Email}\",");
            outputBlob.WriteLine($"\"ProductId\": \"{order.ProductId}\",");
            outputBlob.WriteLine($"\"PurchaseDate\": \"{DateTime.Now}\"");
            outputBlob.Write("}");
        }
    }
}
