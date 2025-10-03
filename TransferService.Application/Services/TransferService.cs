using TransferService.Application.Request;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Interfaces;
using TransferService.Infrastructure.ExternalServices;
using TransferService.Infrastructure.Response;


namespace TransferService.Application.Services
{
    public interface ITransferService
    {
        Task<Transaction> SendMoneyAsync(SendMoneyRequest request);
        Task<Transaction?> WithdrawAsync(WithdrawRequest request);
        Task<Transaction?> CancelTransactionAsync(CancelTransactionRequest request);
        Task CancelPendingTransfersForBlockedCustomerAsync(CustomerBlockedRequest request);
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

        public async Task<Transaction> SendMoneyAsync(SendMoneyRequest request)
        {
            // customer verification
            var sender = await _customerService.VerifyCustomerAsync(request.SenderId);

            var receiver = await _customerService.VerifyCustomerAsync(request.ReceiverId);

            if (sender == null || receiver == null)
                throw new InvalidOperationException("Sender or receiver not found");

            if (sender.Status != CustomerStatus.Active)
                throw new InvalidOperationException("Sender not allowed to send money");

            var transactionId = TransactionIdGenerator();

            // 2. Transaction oluştur -> Pending
            var transaction = new Transaction
            {
                TransactionCode = transactionId,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Amount = request.Amount,
                Status = TransferStatus.Pending,
                Fee = 3,
                Currency = "TRY",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(transaction);

            // Currency exchange
            if (request.Currency != "TRY")
            {
                transaction.USDAmount = request.Amount;
                var rate = await _exchangeService.GetRateAsync(request.Currency, "TRY");
                request.Amount = request.Amount * rate;
            }

            // Daily limit check
            var sentToday = (await _repository.GetBySenderAsync(request.SenderId))
                .Where(t => t.CreatedAt.Date == DateTime.UtcNow.Date && (t.Status == TransferStatus.Pending || t.Status == TransferStatus.Completed))
                .Sum(t => t.Amount);

            if (sentToday + request.Amount > DailyLimit)
            {
                transaction.Status = TransferStatus.Failed;
                await _repository.UpdateAsync(transaction);
                return transaction;
            }

            // Fraud check
            var risk = await _fraudService.CheckRiskAsync(transactionId, request.Amount, request.SenderId, request.ReceiverId, request.Currency);
            if (risk == "HIGH")
            {
                transaction.Status = TransferStatus.Failed;
                await _repository.UpdateAsync(transaction);
                return transaction;
            }


            // Simulate waiting period for amounts >1000 TRY
            if (request.Amount > 1000) await Task.Delay(300_000); // 5 minutes

            return transaction;
        }

        // Örnek format: TXN-20251002-4G7H9K
        public string TransactionIdGenerator()
        {
            var random = new Random();

            // TXN-00001 formatında sahte TransactionId üret
            string transactionId = $"TXN-{random.Next(1000, 9999)}";

            return transactionId;
        }

        public async Task<Transaction?> WithdrawAsync(WithdrawRequest request)
        {
            var transaction = await _repository.GetByCodeAsync(request.TransactionCode);

            if (transaction == null)
                throw new InvalidOperationException("Transaction not found");

            if (transaction.Status != TransferStatus.Pending)
                throw new InvalidOperationException("Transaction not available for withdraw");

            if (transaction.ReceiverId != request.ReceiverId)
                throw new UnauthorizedAccessException("Only receiver can withdraw this transaction");

            // customer verification
            var receiverCustomer = await _customerService.VerifyCustomerAsync(request.ReceiverId);
            if (receiverCustomer == null)
                throw new Exception("Receiver kyc failed");

            transaction.Status = TransferStatus.Completed;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(transaction);

            return transaction;
        }

        public async Task<Transaction?> CancelTransactionAsync(CancelTransactionRequest request)
        {
            var transaction = await _repository.GetByCodeAsync(request.TransactionCode);

            if (transaction == null)
                throw new InvalidOperationException("Transaction not found");

            if (transaction.Status != TransferStatus.Pending)
                throw new InvalidOperationException("Only pending transactions can be cancelled");

            if (transaction.SenderId != request.SenderId)
                throw new UnauthorizedAccessException("Only sender can cancel transaction");

            transaction.Status = TransferStatus.Cancelled;
            transaction.UpdatedAt = DateTime.Now;
            transaction.Fee = 0; // refund fee
            await _repository.UpdateAsync(transaction);
            return transaction;
        }

        public async Task CancelPendingTransfersForBlockedCustomerAsync(CustomerBlockedRequest request)
        {

            var pendingTransactions = await _repository.GetPendingTransactionsByCustomerIdAsync(request.SenderId);

            foreach (var transaction in pendingTransactions)
            {
                transaction.Status = TransferStatus.Cancelled;
                transaction.UpdatedAt = DateTime.Now;
                transaction.Fee = 0; // refund fee
                await _repository.UpdateAsync(transaction);
            }
        }
    }
}