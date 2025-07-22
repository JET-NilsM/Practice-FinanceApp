using System.Data.Entity;
using TransactionService.Data;
using TransactionService.Migrations;
using TransactionService.Models;

namespace TransactionService.Repositories;

/// <summary>
/// Still have to move most of the logic from the controller to this repo
/// </summary>
public class AccountRepository : IAccountRepository
{
    private FinanceContext _context;
    
    public AccountRepository(FinanceContext context)
    {
        _context = context;
    }
    
    public List<Account> GetAccounts()
    {
        return _context.Accounts.ToList();
    }

    public Account GetAccount(int id)
    {
        Account account = _context.Accounts.Find(id);
        if (account != null)
            return account;
        return null;
    }
    
    public void AddAccount(Account account)
    {
        _context.Accounts.Add(account);
        Save();
    }
    
    public void DeleteAccount(int id)
    {
        var account = _context.Accounts.Find(id);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            Save();
        } else throw new KeyNotFoundException($"Account with ID {id} not found.");
    }

    public void UpdateAccount(int id, Account newData)
    {
        var account = _context.Accounts.Find(id);
        if (account == null) 
            return;
        
        account.FullName = newData.FullName;
        account.Email = newData.Email;
        account.Password = newData.Password;
        account.PhoneNumber = newData.PhoneNumber;
        account.Data = new List<AccountData>()
        {
            new AccountData()
            {
                Account = account,
                Balance = newData.Data?.FirstOrDefault()?.Balance ?? 0.0f, // Default to 0 if no balance is provided
                Type = newData.Data?.FirstOrDefault()?.Type ??
                       AccountType.Student // Default to Student if no type is provided
            }
        };
        
        Save();
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}