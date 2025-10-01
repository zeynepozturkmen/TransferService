using System.Transactions;

namespace TransferService.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(Transaction transaction);
        Task<Transaction?> GetByCodeAsync(string code);
        Task<List<Transaction>> GetBySenderAsync(Guid senderId);
        Task UpdateAsync(Transaction transaction);
    }
}
