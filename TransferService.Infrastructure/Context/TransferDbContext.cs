using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace TransferService.Infrastructure.Context
{
    public class TransferDbContext : DbContext
    {
        public TransferDbContext(DbContextOptions<TransferDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
