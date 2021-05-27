using System;
using System.Threading.Tasks;
using AdaRewardsReporter.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace AdaRewardsReporter.Core.Utils
{
    public class BlockfrostClient : ICardanoBlockchainClient
    {
        public readonly string _networkBaseUrl;
        private readonly string _authenticationHeaderKey;
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;
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
            var request = new RestRequest(resourceUri);
            T response;
            try
            {
                response = await _client.GetAsync<T>(request);
            }
            catch (System.Exception exception)
            {
                System.Console.WriteLine($"{exception}");
                throw exception;
            }
            return response;
        }

        private RestClient BuildClient()
        {
            var client = new RestClient(_networkBaseUrl);
            client.AddDefaultHeader(_authenticationHeaderKey, _apiKey);
            return client;
        }
    }
}