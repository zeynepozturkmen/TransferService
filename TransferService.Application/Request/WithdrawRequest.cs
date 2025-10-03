
namespace TransferService.Application.Request
{
    public class WithdrawRequest
    {
        public string TransactionCode { get; set; }
        public Guid ReceiverId { get; set; }
    }
}
