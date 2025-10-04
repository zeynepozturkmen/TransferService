

namespace TransferService.Infrastructure.ExternalServices.Interfaces
{
    public interface IExchangeRateService
    {
        Task<decimal> GetRateAsync(string fromCurrency, string toCurrency);
    }
}
