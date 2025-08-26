using TransactionService.DTO;

namespace TransactionService.Services;

public interface IAccountService
{
    bool AddAccount(AccountDTO dto, string plainTextPassword);
}