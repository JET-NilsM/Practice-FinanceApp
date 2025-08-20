using TransactionService.DTO;
using TransactionService.Models;

namespace TransactionService.Mapper;

public interface IAccountMapper
{
    public Account MapToModel(AccountDTO dto);
}