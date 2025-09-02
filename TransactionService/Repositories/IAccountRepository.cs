using TransactionService.Entities;
using TransactionService.Models;

namespace TransactionService.Repositories;

public interface IAccountRepository : IDisposable
{
    List<AccountEntity> GetAccounts();
    AccountEntity GetAccount(int id);
    AccountEntity AddAccount(AccountEntity entity, Password hashedPassword);
    void DeleteAccount(int id);
    void UpdateAccount(int id, AccountEntity newData);
    Password? GetExistingPassword(int accountID, string hash);
}