using AzureAdCredentialFlowWorker.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzureAdCredentialFlowWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AzureAdOptions options;

        public Worker(ILogger<Worker> logger, AzureAdOptions options)
        {
            _logger = logger;
            this.options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IConfidentialClientApplication app;
                app = ConfidentialClientApplicationBuilder.Create(options.ClientId)
                                                          .WithClientSecret(options.ClientSecret)
                                                          .WithAuthority(new Uri(options.Authority))
                                                          .Build();
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                //Get access token
                var result = await app.AcquireTokenForClient(new string[] { "https://graph.microsoft.com/.default" })
                  .ExecuteAsync();

            }
        }
    }
}
