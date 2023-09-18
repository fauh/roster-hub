using Api.Interfaces;
using BlazorApp.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace Api.Services
{
    public class RosterService : IService<Roster>
    {
        private ILogger _logger;

        private readonly string CosmosDBAccountUri = "https://localhost:8081/";
        private readonly string CosmosDBAccountPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private readonly string CosmosDbName = "rosterhub";
        private readonly string CosmosDbContainerName = "rosters";

        public Container GetContainer()
        {
            var cosmosDbClient = new CosmosClient(CosmosDBAccountUri, CosmosDBAccountPrimaryKey);
            return cosmosDbClient.GetContainer(CosmosDbName, CosmosDbContainerName);
        }

        public RosterService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<HttpStatusCode> AddAsync(Roster model)
        {
            try
            {
                model.Id = Guid.NewGuid();
                var container = GetContainer();
                var response = await container.CreateItemAsync(model);

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return HttpStatusCode.InternalServerError;
            }
        }


        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Roster>> GetAllAsync()
        {
            var rosters = new List<Roster>();
            try
            {
                var container = GetContainer();
                var sqlQuery = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<Roster> queryResultSetIterator = container.GetItemQueryIterator<Roster>(queryDefinition);
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Roster> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (var roster in currentResultSet)
                    {
                        rosters.Add(roster);
                    }
                }

                return rosters;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return rosters;
            }
        }

        public async Task<Roster> GetAsync(string id, string partitionKey)
        {
            try
            {
                var container = GetContainer();
                ItemResponse<Roster> response = await container.ReadItemAsync<Roster>(id, partitionKey: new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task UpdateAsync(Roster model)
        {
            try
            {
                var container = GetContainer();
                ItemResponse<Roster> res = await container.ReadItemAsync<Roster>(Convert.ToString(model.Id), new PartitionKey(model.Faction));

                //Get Existing Item
                var existingItem = res.Resource;

                //Replace existing item values with new values
                string id = Convert.ToString(existingItem.Id);
                var updateRes = await container.ReplaceItemAsync(model, id, new PartitionKey(existingItem.Faction));


            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }
    }
}
