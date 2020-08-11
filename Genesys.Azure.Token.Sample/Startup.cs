using Genesys.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(Genesys.Azure.Token.Sample.Startup))]
namespace Genesys.Azure.Token.Sample
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<GenesysAttributeConfigProvider>();
        }
    }
}
