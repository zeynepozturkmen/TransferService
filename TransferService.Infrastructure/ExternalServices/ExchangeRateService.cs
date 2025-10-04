using TransferService.Infrastructure.ExternalServices.Interfaces;

namespace TransferService.Infrastructure.ExternalServices
{
    public class ExchangeRateService : IExchangeRateService
    {
        public Task<decimal> GetRateAsync(string fromCurrency, string toCurrency)
        {
            if (fromCurrency == "USD" && toCurrency == "TRY") return Task.FromResult(41.61m);
            return Task.FromResult(1m);
        }
    }
}
