using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AzureFunctionDocumentationAzure
{
    public static class PaymentOnReceive
    {
        [FunctionName("PaymentOnReceive")]
        public static async Task<ActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] Order order,
            [Queue("%PaymentQueueName%", Connection = "PaymentQueueConnectionString")] IAsyncCollector<Order> orderQueue,
            [Table("%TableName%", Connection = "PaymentTableConnectionString")] IAsyncCollector<Order> orderTable,
            ILogger log)
        {
            log.LogInformation("Order Received");

            //Add to table
            order.PartitionKey = "orders"; // demo purposed, not for production
            order.RowKey = order.OrderId;
            await orderTable.AddAsync(order);

            //Add to queue
            await orderQueue.AddAsync(order);

            return new OkObjectResult("Thank you for your purchase");
        }
    }

    public class Order: TableEntity
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
    public class TableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
}
