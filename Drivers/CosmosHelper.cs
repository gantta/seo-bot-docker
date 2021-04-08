using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System.Threading.Tasks;

namespace seo_bot_docker {

    public class CosmosHelper {
        CosmosClient _client;
        Container _statsContainer;
        public CosmosHelper(string connectionString) {
            _client = new CosmosClientBuilder(connectionString).WithSerializerOptions(
                new CosmosSerializationOptions {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            ).Build();
            _statsContainer = _client.GetContainer("seo-stats", "search_stats");
        }

        public void AddStat(SearchStats stats) {
            _statsContainer.CreateItemAsync(stats).GetAwaiter().GetResult();
        }
    }
}