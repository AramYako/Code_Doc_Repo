using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace AzureFunctionDocumentationAzure
{
    public static class EmailLincesFile
    {
        [FunctionName("EmailLincesFile")]
        public  static void Run([BlobTrigger("licenses/{orderId}.json", Connection = "BlobStorageConnectionString")]Stream orderBlobStream,
            [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> sender,
            [Table("%TableName%","orders","{orderId}", Connection ="PaymentTableConnectionString")] Order order, //Make lookup in table for us
            string orderId, 
            ILogger log)
        {

            if(order != null)
            {
                log.LogInformation($"EmailLincesFile triggered");

                log.LogInformation($"Got order from {order.Email}");

                var message = new SendGridMessage();

                message.AddTo(order.Email);

                message.SetFrom(new EmailAddress(Environment.GetEnvironmentVariable("EmailSender")));

                message.SetSubject("Your license file");

                message.HtmlContent = "Thank you for your order";

                //if it contains test.com we wont add it, hence to send it
                if (!order.Email.EndsWith("@test.com"))
                    sender.Add(message);
            }
        }
    }
}
