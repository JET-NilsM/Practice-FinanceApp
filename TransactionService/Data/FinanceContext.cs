using Microsoft.EntityFrameworkCore;
using TransactionService.Entities;
using TransactionService.Models;

namespace TransactionService.Data;

public class FinanceContext : DbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    
    public DbSet<Password> HashedPasswords { get; set; }

    public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
    {
        
    }
    
    public bool Save()
    {
        try
        {
            SaveChanges();
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine("An error occurred while saving changes to the database: " + exception.Message);
            return false;
        }

        return true;
    }
}