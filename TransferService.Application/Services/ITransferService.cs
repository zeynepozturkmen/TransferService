using TransferService.Application.Request;
using TransferService.Domain.Entities;

namespace TransferService.Application.Services
{
    public interface ITransferService
    {
        Task<Transaction> SendMoneyAsync(SendMoneyRequest request);
        Task<Transaction?> WithdrawAsync(WithdrawRequest request);
        Task<Transaction?> CancelTransactionAsync(CancelTransactionRequest request);
        Task CancelPendingTransfersForBlockedCustomerAsync(CustomerBlockedRequest request);
    }

}
