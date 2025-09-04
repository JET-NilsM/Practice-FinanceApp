using TransactionService.DTO;
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
        string hash = PasswordHasher.HashPassword(model.Password);
        // if (_repo.GetExistingPassword(model.ID, hash) != null)
        //     return false;
        
        AccountEntity entity = AccountMapper.ModelToEntity(model);
        Password password = new Password()
        {
            AccountID = entity.ID,
            HashedPassword = hash,
            CreatedAt = DateTime.UtcNow
        };

        try
        { 
            _repo.AddAccount(entity, password);
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

    public AccountModel GetAccount(int id)
    {
        AccountEntity accountEntity = _repo.GetAccount(id);
        if (accountEntity == null)
            return null;
        
        AccountModel accountModel = AccountMapper.EntityToModel(accountEntity);
        return accountModel;
    }

    public void DeleteAccount(int id)
    {
        _repo.DeleteAccount(id);
    }

    public void UpdateAccount(int id, AccountModel newData)
    {
        AccountEntity newDataEntity = AccountMapper.ModelToEntity(newData);
        _repo.UpdateAccount(id, newDataEntity);
    }
}