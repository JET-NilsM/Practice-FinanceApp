using TransactionService.DTO;
using TransactionService.Mapper;
using TransactionService.Models;
using TransactionService.Repositories;

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
    
    public bool AddAccount(AccountDTO dto)
    {
        //Error handling?
        Account accountModel = _mapper.MapToModel(dto);
        return _repo.AddAccount(accountModel);
    }
}