using System.Net.Http.Json;
using TransferService.Infrastructure.Response;

namespace TransferService.Infrastructure.ExternalServices
{
    public interface IFraudDetectionService
    {
        Task<string> CheckRiskAsync(string transactionId, decimal amount, Guid senderId, Guid receiverId, string currency);
    }

    public class FraudDetectionService : IFraudDetectionService
    {

        private readonly HttpClient _httpClient;
        public FraudDetectionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CheckRiskAsync(string transactionId,decimal amount, Guid senderId, Guid receiverId, string currency)
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

    public interface IExchangeRateService
    {
        Task<decimal> GetRateAsync(string fromCurrency, string toCurrency);
    }

    public class ExchangeRateService : IExchangeRateService
    {
        public Task<decimal> GetRateAsync(string fromCurrency, string toCurrency)
        {
            if (fromCurrency == "USD" && toCurrency == "TRY") return Task.FromResult(41.61m);
            return Task.FromResult(1m);
        }
    }

    public interface ICustomerService
    {
        Task<CustomerResult?> VerifyCustomerAsync(Guid customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;
        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerResult?> VerifyCustomerAsync(Guid customerId)
        {

            // Customer servisi doğrulama islemi, base url örneği:
            //https://localhost:44322/api/Customers
            var response = await _httpClient.GetAsync($"https://localhost:44322/api/Customers/{customerId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<CustomerResult>();
            return result;
        }
    }

}
