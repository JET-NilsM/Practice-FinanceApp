using TransactionService.Models;

namespace TransactionService.Repositories;

public interface IAccountRepository : IDisposable
{
    List<Account> GetAccounts();
    Account GetAccount(int id);
    void AddAccount(Account account);
    void DeleteAccount(int id);
    void UpdateAccount(int id, Account newData);
}