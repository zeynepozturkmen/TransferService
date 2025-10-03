using Microsoft.EntityFrameworkCore;
using TransferService.Domain.Entities;
using TransferService.Domain.Enums;
using TransferService.Domain.Interfaces;
using TransferService.Infrastructure.Context;

namespace TransferService.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransferDbContext _context;
        public TransactionRepository(TransferDbContext context) => _context = context;

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetByCodeAsync(string code)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionCode == code);
        }

        public async Task<List<Transaction>> GetBySenderAsync(Guid senderId)
        {
            return await _context.Transactions.Where(t => t.SenderId == senderId).ToListAsync();
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetPendingTransactionsByCustomerIdAsync(Guid senderId)
        {
            return await _context.Transactions
                .Where(t => t.SenderId == senderId && t.Status == TransferStatus.Pending)
                .ToListAsync();
        }

    }

}
