using System.Data.Entity;
using Microsoft.AspNetCore.Components.Web;
using TransactionService.Data;
using TransactionService.Entities;
using TransactionService.Models;

namespace TransactionService.Repositories;

/// <summary>
/// Still have to move most of the logic from the controller to this repo
/// </summary>
public class AccountRepository : IAccountRepository
{
    private FinanceContext _context;
    private readonly ILogger _logger;
    
    public AccountRepository(FinanceContext context, ILogger<AccountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public List<AccountEntity> GetAccounts()
    {
        return _context.Accounts.ToList();
    }

    public AccountEntity? GetAccount(int id)
    {
        AccountEntity accountEntity;
        try
        {
            accountEntity = _context.Accounts.Find(id);
        }
        catch (Exception e)
        {
            _logger.LogError("Error when retrieving account: " + e.Message);
            return null;
        }

        return accountEntity;
    }
    
    public AccountEntity? AddAccount(AccountEntity entity, Password hashedPassword)
    {
        _context.HashedPasswords.Add(hashedPassword);       
        _context.Accounts.Add(entity);
        if(Save())
            return entity;
        return null;
    }
    
    public void DeleteAccount(int id)
    {
        var account = _context.Accounts.Find(id);
        if (account != null)
        {
            _context.Accounts.Remove(account);
        } else throw new KeyNotFoundException($"Account with ID {id} not found.");

        Save();
    }

    public AccountEntity? UpdateAccount(int id, AccountEntity newData)
    {
        var account = _context.Accounts.Find(id);
        if (account == null)
            return null;
        
        account.FullName = newData.FullName;
        account.Email = newData.Email;
        account.PhoneNumber = newData.PhoneNumber;

        if(Save())
            return account;
        return null;
    }

    public Password? GetExistingPassword(int accountID, Password password)
    {
        _logger.LogInformation("Reached GetExistingPassword() method in AccountRepository.");
        try
        {
            var oldPasswordsForAccount = _context.HashedPasswords.Where(stored => stored.AccountID == accountID);
            foreach (var existingPassword in oldPasswordsForAccount)
            {
                if (existingPassword.HashedPassword == password.HashedPassword)
                    return password;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Error when retrieving existing password: " + e.Message);
            return null;
        }

        return null;
    }

    public bool Save()
    {
        try
        {
            _context.Save();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error occured when saving to database: " + e);
            return false;
        }
        
        return true;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}