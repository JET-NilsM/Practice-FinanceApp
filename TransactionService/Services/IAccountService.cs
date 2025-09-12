using TransactionService.Models;

namespace TransactionService.Services;

public interface IAccountService
{ 
    bool AddAccount(AccountModel model);
    List<AccountModel> GetAccounts();
    AccountModel? GetAccount(int id);
    bool DeleteAccount(int id);
    bool UpdateAccount(int id, AccountModel model);
}