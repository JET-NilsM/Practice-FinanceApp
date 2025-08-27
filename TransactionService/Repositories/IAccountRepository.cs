using TransactionService.Models;

namespace TransactionService.Repositories;

public interface IAccountRepository : IDisposable
{
    List<Account> GetAccounts();
    Account GetAccount(int id);
    bool AddAccount(Account account, Password hashedPassword);
    void DeleteAccount(int id);
    void UpdateAccount(int id, Account newData);
}