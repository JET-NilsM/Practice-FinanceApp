using TransactionService.DTO;
using TransactionService.Models;

namespace TransactionService.Services;

public interface IAccountService
{ 
    bool AddAccount(AccountModel model);
    List<AccountModel> GetAccounts();
    AccountModel GetAccount(int id);
    void DeleteAccount(int id);
    void UpdateAccount(int id, AccountModel model);
}