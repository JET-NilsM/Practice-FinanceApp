using Microsoft.EntityFrameworkCore;
using TransactionService.Models;

namespace TransactionService.Data;

public class FinanceContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
    {
        
    }
}