using TransactionService.DTO;
using TransactionService.Mapper;
using TransactionService.Models;
using TransactionService.Repositories;
using TransactionService.Utilities;

namespace TransactionService.Services;

public class AccountService : IAccountService
{
    private IAccountRepository _repo;
    private IAccountMapper _mapper;

    public AccountService(IAccountRepository repo, IAccountMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    
    public bool AddAccount(AccountDTO dto, string plainTextPassword)
    {
        string hash = PasswordHasher.HashPassword(plainTextPassword);
        Account accountModel = _mapper.MapToModel(dto);
        Password password = new Password()
        {
            AccountID = accountModel.ID,
            HashedPassword = hash,
            CreatedAt = DateTime.UtcNow
        };
        
        return _repo.AddAccount(accountModel, password);
    }
}