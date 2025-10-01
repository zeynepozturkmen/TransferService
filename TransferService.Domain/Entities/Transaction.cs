using TransferService.Domain.Enums;

namespace TransferService.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public decimal Fee { get; set; }
        public string TransactionCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        public TransferStatus Status { get; set; } = TransferStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
