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

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1yNS1BVWliZkJpaTdOZDFqQmViYXhib1hXMCIsImtpZCI6Ik1yNS1BVWliZkJpaTdOZDFqQmViYXhib1hXMCJ9.eyJhdWQiOiJhcGk6Ly81YTAxMWNhNi0yYzg2LTRkMGUtOWYwMS1jNDM2NjkyYWJkNmUiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83YzJhYTNlNC1kMzg2LTQ1ZGQtYTdlNy1iZWRjOTIxY2ZlMDEvIiwiaWF0IjoxNjQwOTExMjAxLCJuYmYiOjE2NDA5MTEyMDEsImV4cCI6MTY0MDkxNTM1MSwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFZUUFlLzhUQUFBQUhsSEVEUTBhbWs3OFN3QTJ1MUE3RzBzamxWWG56SG1FbmpNQjU1WCtiNXJjYkttMWNoM250cGRZdmRHcEtPUVVYRUZMdHFmc1RDZ1BGcE8xcWw2Q0hIeXJLNHdRS2xnamt2V1RKUDFBU0gxcysrQ0NZVm1vMENhdHRoL3lPVCs3YytlS0JCMG1rMlJWK0Jsb3d3MThpNkFQbXl5SVorRDVlR1hoWjNZeXhNYz0iLCJhbXIiOlsicHdkIiwibWZhIl0sImFwcGlkIjoiN2Q5N2ZhNDUtNzc4NC00YTk1LWFlMzUtMTc1NjJjY2M3M2QzIiwiYXBwaWRhY3IiOiIxIiwiY3RyeSI6IlNFIiwiZW1haWwiOiJBcmFteWFrb0Bob3RtYWlsLmNvbSIsImZhbWlseV9uYW1lIjoiWWFrbyIsImdpdmVuX25hbWUiOiJBcmFteWFrb0Bob3RtYWlsLmNvbSIsImdyb3VwcyI6WyI1NjI4OTgyNS0xNzU4LTRlYTItOThkZC04YWFjMGIxMzUwOTAiLCIzZGI0Y2IyNS0yYTBhLTQ2YTEtODE4OS1iZjExMjY2YjVmNTAiLCIxY2IxZDU0MC1hNzBjLTRhMjMtYjBmYi1hMTE2NDc1ZjAzMWQiXSwiaWRwIjoibGl2ZS5jb20iLCJpcGFkZHIiOiI4My4yNDkuMTg4LjE1NCIsIm5hbWUiOiJBcmFteWFrb0Bob3RtYWlsLmNvbSBZYWtvIiwib2lkIjoiZTBkMTQ3NGUtZDA4Ny00ZmFkLTk3OWQtNTc4MTFmOGJiYzc2IiwicmgiOiIwLkFVOEE1S01xZkliVDNVV241Nzdja2h6LUFVWDZsMzJFZDVWS3JqVVhWaXpNYzlOUEFKdy4iLCJyb2xlcyI6WyJBZG1pbiJdLCJzY3AiOiJGaWxlcy5SZWFkIE1hbmFnZW1lbnQiLCJzdWIiOiJZREJncmNaMWFqcW1pcWdUSXg3NU1iRmpCSWVKa3VSVkZkY2pnRmVmRUVzIiwidGVuYW50X2N0cnkiOiJTRSIsInRpZCI6IjdjMmFhM2U0LWQzODYtNDVkZC1hN2U3LWJlZGM5MjFjZmUwMSIsInVuaXF1ZV9uYW1lIjoibGl2ZS5jb20jQXJhbXlha29AaG90bWFpbC5jb20iLCJ1dGkiOiJ6YktUZ1JYbjQwNi1POVdWcDl3ZkFnIiwidmVyIjoiMS4wIn0.FnykGt4hHOmsjf_haeuUpigmVqRztWmEFQkkteslb8_Mecm9vSucdM6DHKQIJi2wUg4hdFwU1p8Sg_6ZkXgC5Ko3yZpuyO0JglD_Q_weHgovq6Nn6YCaS3X2Hcjlntozy5gYSWWJhVbrgHyw4yWKJabS9cAWcb47Dz3Ts6wLEs2fcMlRWpAD8EDbHQXT8ewMUc1x139b6tvAsZhmbtmcRMa5gADRjjgNQ_A30l8aFhwYXcHl5vPJikk4HVWh7wzMDw_raGi-KXZpuqRArtRItP-d2ywrOBK40zKGuvupHa2uiFJCLBhOZXoF2kkwHWO8mSywVkg_LSViT5lPwQAH2g");

                    //Group authorization
                    var httpResultGroup = await httpClient.GetAsync("https://localhost:7298/api/GroupAuthorization/Group");

                    //Group authorization
                    var httpResultGroup2 = await httpClient.GetAsync("https://localhost:7298/api/GroupAuthorization/GroupStartUp");

                    //Role Management GroupStartUp
                    var httpResult = await httpClient.GetAsync("https://localhost:7298/api/Role/Adminy");

                    //Scope Management
                    var httpResult2 = await httpClient.GetAsync("https://localhost:7298/api/Scope/GetScope");

                    //Scopes Management
                    var httpResult3 = await httpClient.GetAsync("https://localhost:7298/api/Scope/GetScopes");
                }
            }
            catch(MsalClientException ex)
            {
                log.LogError(ex.Message);
            }

            return new OkObjectResult("ok");
        }
    }
}