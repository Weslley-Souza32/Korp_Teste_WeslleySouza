using System.Net;
using System.Net.Http.Json;
using Korp.Billing.Api.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

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

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(
                    requestUri,
                    cancellationToken);
            }
            catch (HttpRequestException exception)
            {
                throw new ServiceUnavailableException(
                    "Stock service is currently unavailable.",
                    exception);
            }
            catch (TimeoutException exception)
            {
                throw new ServiceUnavailableException(
                    "Stock service did not respond within the expected time.",
                    exception);
            }

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

        public async Task<DebitStockResponse> DebitStockAsync(
            DebitStockRequest request,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync(
                    "api/stock/debit",
                    request,
                    cancellationToken);
            }
            catch (HttpRequestException exception)
            {
                throw new ServiceUnavailableException(
                    "Stock service is currently unavailable.",
                    exception);
            }
            catch (TimeoutException exception)
            {
                throw new ServiceUnavailableException(
                    "Stock service did not respond within the expected time.",
                    exception);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var problemDetails = await response.Content
                    .ReadFromJsonAsync<ProblemDetails>(
                        cancellationToken: cancellationToken);

                throw new NotFoundException(
                    problemDetails?.Detail
                    ?? "A product required by the invoice was not found.");
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var problemDetails = await response.Content
                    .ReadFromJsonAsync<ProblemDetails>(
                        cancellationToken: cancellationToken);

                throw new ConflictException(
                    problemDetails?.Detail
                    ?? "Stock could not be debited.");
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<DebitStockResponse>(
                    cancellationToken: cancellationToken);

            return result
                ?? throw new InvalidOperationException(
                    "Stock service returned an empty response.");
        }
    }
}