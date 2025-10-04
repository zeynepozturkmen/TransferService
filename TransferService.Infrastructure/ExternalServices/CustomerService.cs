using System.Net.Http.Json;
using TransferService.Infrastructure.ExternalServices.Interfaces;
using TransferService.Infrastructure.Response;

namespace TransferService.Infrastructure.ExternalServices
{
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
            // HttpRequestMessage oluştur
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:44322/api/Customers/{customerId}");

            // API Key header ekle
            request.Headers.TryAddWithoutValidation("X-Api-Key", "my-secret-api-key-123");

            // İstek gönder
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<CustomerResult>();
            return result;
        }
    }
}
