using TransactionService.DTO;
using TransactionService.Entities;
using TransactionService.Models;

namespace TransactionService.Mapper;

public static class AccountMapper
{
    public static AccountModel DtoToModel(AccountDTO dto)
    {
        if (dto == null)
        {
            throw new NullReferenceException("Account DTO is null");
        }
        
        return new AccountModel
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Password = dto.Password,
            PhoneNumber = dto.PhoneNumber
        };
    }
    
    public static AccountModel EntityToModel(AccountEntity entity)
    {
        if (entity == null)
        {
            throw new NullReferenceException("Account entity is null");
        }
        
        return new AccountModel
        {
            ID = entity.ID,
            FullName = entity.FullName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber
        };
    }
    
    public static AccountDTO ModelToDto(AccountModel model)
    {
        if (model == null)
        {
            throw new NullReferenceException("Account model is null");
        }
        
        return new AccountDTO
        {
            ID = model.ID,
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };
    }
    
    public static AccountEntity ModelToEntity(AccountModel model)
    {
        if (model == null)
        {
            throw new NullReferenceException("Account model is null");
        }
        
        return new AccountEntity
        {
            ID = model.ID,
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };
    }
}