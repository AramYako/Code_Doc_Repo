using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Linq;

namespace ClientCredentialFlowService
{
    public class ClientCredentialFlow
    {
        public AzureOptions options { get; set; }
        public ClientCredentialFlow(IOptions<AzureOptions> azureOptions)
        {
            this.options = new AzureOptions()
            {
                ResourceId = Environment.GetEnvironmentVariable(nameof(AzureOptions.ResourceId)),
                TenantId = Environment.GetEnvironmentVariable(nameof(AzureOptions.TenantId)),
                ClientId = Environment.GetEnvironmentVariable(nameof(AzureOptions.ClientId)),
                ClientSecret = Environment.GetEnvironmentVariable(nameof(AzureOptions.ClientSecret)),
                Instance = Environment.GetEnvironmentVariable(nameof(AzureOptions.Instance)),
            };
        }

        [FunctionName("ClientCredentialFlowService")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(options.ClientId)
                                                      .WithClientSecret(options.ClientSecret)
                                                      .WithAuthority(new Uri(options.Authority))
                                                      .Build();

            try
            {
                var result = await app.AcquireTokenForClient(new string[] { options.ResourceId })
                    .ExecuteAsync();

                if(result?.AccessToken != null)
                {
                    var httpClient = new HttpClient();

                    if (httpClient.DefaultRequestHeaders.Accept == null || !httpClient.DefaultRequestHeaders.Accept.Any(x => x.MediaType == "application/json"))
                        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

                    //No scope
                    var httpResult = await httpClient.GetAsync("https://localhost:7298/api/Values");

                    //Scope Management
                    var httpResult2 = await httpClient.GetAsync("https://localhost:7298/api/Values/GetValue");

                    //Scope Management & Files.Read
                    var httpResult3 = await httpClient.GetAsync("https://localhost:7298/api/Values/GetValues");
                }
            }
            catch(MsalClientException ex)
            { 

            }

            return new OkObjectResult("ok");
        }
    }
}
