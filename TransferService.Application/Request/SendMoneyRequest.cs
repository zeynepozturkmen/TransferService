

namespace TransferService.Application.Request
{
    public class SendMoneyRequest
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
