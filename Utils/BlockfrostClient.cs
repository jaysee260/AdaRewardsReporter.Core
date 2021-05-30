using System;
using System.Threading.Tasks;
using AdaRewardsReporter.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace AdaRewardsReporter.Core.Utils
{
    public class BlockfrostClient : ICardanoBlockchainClient
    {
        private readonly string _networkBaseUrl;
        private readonly string _authenticationHeaderKey;
        private readonly string _apiKey;
        private readonly RestClient _client;

        public BlockfrostClient(IConfiguration configuration)
        {
            _networkBaseUrl = configuration.GetValue<string>("MainnetBaseUrl") ?? throw new ArgumentNullException("MainnetBaseUrl missing");
            _authenticationHeaderKey = configuration.GetValue<string>("AuthenticationHeaderKey") ?? throw new ArgumentNullException("AuthenticationHeaderKey missing");
            _apiKey = configuration.GetValue<string>("ApiKey") ?? throw new ArgumentNullException("ApiKey missing");
            _client = BuildClient();
        }

        public async Task<T> QueryAsync<T>(string resourceUri)
        {
            try
            {
                var request = new RestRequest(resourceUri);
                var response = await _client.GetAsync<T>(request);
                return response;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception thrown in {nameof(BlockfrostClient)}.{nameof(QueryAsync)}");
                Console.WriteLine($"{exception}");
                throw;
            }
        }

        private RestClient BuildClient()
        {
            var client = new RestClient(_networkBaseUrl);
            client.AddDefaultHeader(_authenticationHeaderKey, _apiKey);
            return client;
        }
    }
}