
namespace TransferService.Infrastructure.ExternalServices.Interfaces
{
    public interface IFraudDetectionService
    {
        Task<string> CheckRiskAsync(string transactionId, decimal amount, Guid senderId, Guid receiverId, string currency);
    }
}
