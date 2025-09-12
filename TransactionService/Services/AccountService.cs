using TransactionService.Entities;
using TransactionService.Mapper;
using TransactionService.Models;
using TransactionService.Repositories;
using TransactionService.Utilities;

namespace TransactionService.Services;

public class AccountService : IAccountService
{
    private IAccountRepository _repo;
    private readonly ILogger _logger;

    public AccountService(IAccountRepository repo, ILogger<IAccountService> logger)
    {
        _repo = repo;
    }
    
    public bool AddAccount(AccountModel model)
    {
        Password hashedPasswordData = PasswordHasher.HashPassword(model.Password);
        AccountEntity entity = AccountMapper.ModelToEntity(model);
        hashedPasswordData.AccountID = entity.ID;
        hashedPasswordData.CreatedAt = DateTime.Now;

        try
        { 
            _repo.AddAccount(entity, hashedPasswordData);
        }
        catch (Exception e)
        {
            _logger.LogError("Error when adding account: " + e.Message);
            return false;    
        }

        return true;
    }

    public List<AccountModel> GetAccounts()
    {
        List<AccountModel> accountModels = new List<AccountModel>();
        
        List<AccountEntity> entities = _repo.GetAccounts();
        foreach (var entity in entities)
        {
            accountModels.Add(AccountMapper.EntityToModel(entity));
        }
        return accountModels;
    }

    public AccountModel? GetAccount(int id)
    {
        AccountEntity accountEntity = _repo.GetAccount(id);
        if (accountEntity == null)
            return null;
        
        AccountModel accountModel = AccountMapper.EntityToModel(accountEntity);
        return accountModel;
    }

    public bool DeleteAccount(int id)
    {
        try
        {
            _repo.DeleteAccount(id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error when deleting account: " + e.Message);
            return false;
        }

        return true;
    }

    public bool UpdateAccount(int id, AccountModel newData)
    {
        try
        {
            if (!string.IsNullOrEmpty(newData.Password))
            {
                Password password = PasswordHasher.HashPassword(newData.Password);
                if (_repo.GetExistingPassword(id, password) != null)
                    return false;
            }

            AccountEntity newDataEntity = AccountMapper.ModelToEntity(newData);
            _repo.UpdateAccount(id, newDataEntity);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error when updating account: " + e.Message);
            return false;
        }

        return true;
    }
}