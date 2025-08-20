using TransactionService.DTO;
using TransactionService.Models;

namespace TransactionService.Mapper;

public class AccountMapper : IAccountMapper
{
    public Account MapToModel(AccountDTO dto)
    {
        return new Account
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };
    }
}