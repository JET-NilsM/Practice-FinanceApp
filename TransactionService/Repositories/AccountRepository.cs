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
    
    public AccountRepository(FinanceContext context)
    {
        _context = context;
    }
    
    public List<AccountEntity> GetAccounts()
    {
        return _context.Accounts.ToList();
    }

    public AccountEntity GetAccount(int id)
    {
        AccountEntity account = _context.Accounts.Find(id);
        if (account != null)
            return account;
        return null;
    }
    
    public AccountEntity AddAccount(AccountEntity entity, Password hashedPassword)
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

    public void UpdateAccount(int id, AccountEntity newData)
    {
        var account = _context.Accounts.Find(id);
        if (account == null) 
            return;
        
        account.FullName = newData.FullName;
        account.Email = newData.Email;
        account.PhoneNumber = newData.PhoneNumber;

        Save();
    }

    public Password? GetExistingPassword(int accountID, string incomingHash)
    {
        var oldPasswordsForAccount = _context.HashedPasswords.Where(stored => stored.AccountID == accountID);
        foreach (var password in oldPasswordsForAccount)
        {
            if (password.HashedPassword == incomingHash)
                return password;
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