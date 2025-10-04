using System.Net.Http.Json;
using TransferService.Infrastructure.ExternalServices.Interfaces;
using TransferService.Infrastructure.Response;

namespace TransferService.Infrastructure.ExternalServices
{
    public class FraudDetectionService : IFraudDetectionService
    {

        private readonly HttpClient _httpClient;
        public FraudDetectionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CheckRiskAsync(string transactionId, decimal amount, Guid senderId, Guid receiverId, string currency)
        {
            var payload = new
            {
                transactionId = transactionId,
                userId = senderId,
                toUserId = receiverId,
                amount = amount,
                currency = currency,

            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:8081/api/fraud/check", payload);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<FraudResponse>();
            return result?.Data?.RiskLevel ?? "HIGH"; // default HIGH risk
        }
    }

}
