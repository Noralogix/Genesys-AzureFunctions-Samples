using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;

namespace GenesysApiFunctionApp
{
    public static class ConversationsAggregates
    {
        private static ApiClient apiClient = new ApiClient($"https://api.mypurecloud.ie");

        [FunctionName("ConversationsAggregates")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "conversations/aggregates")] HttpRequest req,
            ILogger log)
        {
            try
            {                
                var configuration = new Configuration(apiClient);

                var purecloudClientId = System.Environment.GetEnvironmentVariable("ClientId");
                var purecloudClientSecret = System.Environment.GetEnvironmentVariable("ClientSecret");
                var authTokenInfo = apiClient.PostToken(purecloudClientId, purecloudClientSecret);
                configuration.AccessToken = authTokenInfo.AccessToken;

                var analyticsApi = new AnalyticsApi(configuration);
                var dtUtc = DateTime.Now.ToUniversalTime();
                var dtFrom = dtUtc.AddDays(-7);
                var dtTo = dtUtc;
                var response = await analyticsApi.PostAnalyticsConversationsAggregatesQueryAsync(new ConversationAggregationQuery
                {
                    Interval = $"{dtFrom:O}/{dtTo:O}",
                });
                return new OkObjectResult(new { Items = response?.Results });
            }
            catch (ApiException ex)
            {
                log.LogError(ex, "Purecloud error");
                return new BadRequestResult();
            }
        }
    }
}
