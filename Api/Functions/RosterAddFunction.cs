using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Api.Services;
using ApiIsolated;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace Api.Functions
{
    public class RosterAddFunction
    {
        private ILogger<RosterAddFunction> _logger;

        public RosterAddFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RosterAddFunction>();
        }

        [Function("AddRoster")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] string json, HttpRequestData req)
        {

            var rosterService = new RosterService(_logger);
            // deserialize json to a Roster model
            try
            {
                var model = JsonSerializer.Deserialize<Roster>(json);

                var responseStatusCode = await rosterService.AddAsync(model);

                var response = req.CreateResponse(responseStatusCode);
                await response.WriteAsJsonAsync(response);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

           
        }
    }
}
