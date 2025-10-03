

namespace TransferService.Application.Request
{
    public class CancelTransactionRequest
    {
        public string TransactionCode { get; set; }
        public Guid SenderId { get; set; }
    }
}
