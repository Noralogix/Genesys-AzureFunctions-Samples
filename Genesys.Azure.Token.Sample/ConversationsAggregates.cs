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
using Genesys.Azure.WebJobs.Extensions;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;

namespace Genesys.Azure.Token.Sample
{
    public static class ConversationsAggregates
    {
        [FunctionName("ConversationsAggregates")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "conversations/aggregates")] HttpRequest req
            , [Genesys] IGenesysAccessToken token
            , ILogger log)
        {            
            try
            {
                ApiClient apiClient = new ApiClient($"https://api.{token.Environment}");
                var configuration = new Configuration(apiClient);
                configuration.AccessToken = token.Value;

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
