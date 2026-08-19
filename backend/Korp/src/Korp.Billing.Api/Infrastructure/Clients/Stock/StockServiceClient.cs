using System.Net;

namespace Korp.Billing.Api.Infrastructure.Clients.Stock
{
    public class StockServiceClient : IStockServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StockServiceClient> _logger;

        public StockServiceClient(
            HttpClient httpClient,
            ILogger<StockServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<StockProductResponse?> GetProductByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var requestUri = $"api/products/{productId}";

            _logger.LogInformation(
                "Calling Stock Service: {BaseAddress}{RequestUri}",
                _httpClient.BaseAddress,
                requestUri);

            var response = await _httpClient.GetAsync(
                requestUri,
                cancellationToken);

            _logger.LogInformation(
                "Stock Service returned status code {StatusCode}",
                response.StatusCode);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<StockProductResponse>(
                cancellationToken: cancellationToken);
        }
    }
}