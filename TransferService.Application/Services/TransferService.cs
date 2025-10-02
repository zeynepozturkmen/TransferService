using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Interfaces;
using TransferService.Infrastructure.ExternalServices;

namespace TransferService.Application.Services
{
    public interface ITransferService
    {
        Task<Transaction> SendMoneyAsync(Guid senderId, Guid receiverId, decimal amount, string currency);
        Task<Transaction?> ReceiveMoneyAsync(string transactionCode, Guid receiverId);
        Task<Transaction?> CancelTransactionAsync(string transactionCode);
    }

    public class TransferServiceApp : ITransferService
    {
        private readonly ITransactionRepository _repository;
        private readonly IFraudDetectionService _fraudService;
        private readonly IExchangeRateService _exchangeService;
        private readonly ICustomerService _customerService;
        private const decimal DailyLimit = 10000;

        public TransferServiceApp(
            ITransactionRepository repository,
            IFraudDetectionService fraudService,
            IExchangeRateService exchangeService,
            ICustomerService customerService)
        {
            _repository = repository;
            _fraudService = fraudService;
            _exchangeService = exchangeService;
            _customerService = customerService;
        }

        public async Task<Transaction> SendMoneyAsync(Guid senderId, Guid receiverId, decimal amount, string currency)
        {
            // customer verification
            var senderCustomer = await _customerService.VerifyCustomerAsync(senderId);

            if (senderCustomer == null)
                throw new Exception("sender kyc failed");


            // customer verification
            var receiverCustomer = await _customerService.VerifyCustomerAsync(receiverId);
            if (receiverCustomer == null)
                throw new Exception("receiver kyc failed");

            // Currency exchange
            var usdAmount = 0.0m;
            if (currency != "TRY")
            {
                usdAmount = amount;
                var rate = await _exchangeService.GetRateAsync(currency, "TRY");
                amount = amount * rate;
            }

            // Daily limit check
            var sentToday = (await _repository.GetBySenderAsync(senderId))
                .Where(t => t.CreatedAt.Date == DateTime.UtcNow.Date)
                .Sum(t => t.Amount);
            if (sentToday + amount > DailyLimit)
                throw new Exception("Daily transfer limit exceeded");

            var transactionId = TransactionIdGenerator();

            // Fraud check
            var risk = await _fraudService.CheckRiskAsync(transactionId, amount, senderId, receiverId, currency);
            if (risk == "HIGH") throw new Exception("Transaction rejected due to high risk");

            var transaction = new Transaction
            {
                TransactionCode = transactionId,
                SenderId = senderId,
                ReceiverId = receiverId,
                Amount = amount,
                Currency = "TRY",
                USDAmount = usdAmount,
                Status = TransferStatus.Pending,
                Fee = 3
            };

            // Simulate waiting period for amounts >1000 TRY
            if (amount > 1000) await Task.Delay(300_000); // 5 minutes

            transaction.Status = TransferStatus.Completed;
            transaction.CompletedAt = DateTime.UtcNow;

            return await _repository.AddAsync(transaction);
        }

        // Örnek format: TXN-20251002-4G7H9K
        public  string TransactionIdGenerator()
        {
            var random = new Random();

            // TXN-00001 formatında sahte TransactionId üret
            string transactionId = $"TXN-{random.Next(1000, 9999)}";

            return transactionId;
        }

        public async Task<Transaction?> ReceiveMoneyAsync(string transactionCode, Guid receiverId)
        {
            var transaction = await _repository.GetByCodeAsync(transactionCode);
            if (transaction == null || transaction.ReceiverId != receiverId || transaction.Status != TransferStatus.Completed)
                return null;

            // customer verification
            var receiverCustomer = await _customerService.VerifyCustomerAsync(receiverId);
            if (receiverCustomer == null)
                throw new Exception("receiver kyc failed");


            transaction.Status = TransferStatus.Closed;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(transaction);

            return transaction;
        }

        public async Task<Transaction?> CancelTransactionAsync(string transactionCode)
        {
            var transaction = await _repository.GetByCodeAsync(transactionCode);
            if (transaction == null || transaction.Status == TransferStatus.Closed)
                return null;

            transaction.Status = TransferStatus.Cancelled;
            transaction.UpdatedAt = DateTime.Now;
            transaction.Fee = 0; // refund fee
            await _repository.UpdateAsync(transaction);
            return transaction;
        }
    }
}