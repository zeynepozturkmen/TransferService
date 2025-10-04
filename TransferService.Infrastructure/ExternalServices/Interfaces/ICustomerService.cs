using TransferService.Infrastructure.Response;

namespace TransferService.Infrastructure.ExternalServices.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResult?> VerifyCustomerAsync(Guid customerId);
    }
}
