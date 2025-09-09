using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Data;
using TransactionService.DTO;
using TransactionService.Mapper;
using TransactionService.Models;
using TransactionService.Repositories;
using TransactionService.Services;
using TransactionService.Utilities;
using Xunit.Sdk;

namespace TransactionService.Controllers;

[ApiController] //or [FromBody] if it should only be used for specific methods
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private IAccountService _service;
    private readonly ILogger _logger;

    public AccountController(IAccountService service, ILogger<AccountController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(AccountDTO? incomingData)
    {
        _logger.LogInformation("---- Reached the PUT account method ----");

        if (!ModelState.IsValid)
            return BadRequest(); 

        if(incomingData == null)
            return BadRequest();

        AccountModel newAccountModel = AccountMapper.DtoToModel(incomingData);

        if (!_service.AddAccount(newAccountModel))
            return BadRequest();

        AccountDTO accountDto = AccountMapper.ModelToDto(newAccountModel);
        return Created("/api/account", accountDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        _logger.LogInformation("---- Reached the GET account method ----");

        AccountModel selectedAccountModel = _service.GetAccount(id);
        if(selectedAccountModel == null)
            return NotFound();
        AccountDTO accountDto = AccountMapper.ModelToDto(selectedAccountModel);

        return Ok(accountDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        _logger.LogInformation("---- Reached the GET accounts method ----");

        List<AccountModel> accounts = _service.GetAccounts();
        if (accounts == null)
            return NotFound();

        if (accounts.Count == 0)
            return NotFound();

        return Ok(accounts);
    }

    [HttpPut("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, AccountDTO accountDto)
    {
        _logger.LogInformation("---- Reached the PUT account method ----");

        AccountModel selectedAccountModel = _service.GetAccount(givenID);
        if (selectedAccountModel == null)
            return NotFound();

        AccountModel accountModel = AccountMapper.DtoToModel(accountDto);
        if(!_service.UpdateAccount(givenID, accountModel))
            return BadRequest();

        return Ok(accountDto);
    }

    [HttpPatch("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Dictionary<string, object> newData)
    {
        _logger.LogInformation("---- Reached the PATCH account method ----");

        AccountModel? existingAccountModel = _service.GetAccount(givenID);
        if (existingAccountModel == null)
            return NotFound();

        if (!newData.Any())
            return BadRequest();
        
        AccountDTO accountDto = AccountMapper.ModelToDto(existingAccountModel);
        foreach (var dataEntry in newData)
        {
            if (dataEntry.Key == "ID")
                continue;

            object? convertedValue;
            try
            {
                convertedValue = AccountHelper.ConvertToProperty(dataEntry, accountDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting property: " + ex.Message);
                return BadRequest();
            }

            if (convertedValue == null)
                return BadRequest(); 
        }
        
        AccountModel newAccountModel = AccountMapper.DtoToModel(accountDto);
        if(!_service.UpdateAccount(givenID, newAccountModel))
            return BadRequest();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        _logger.LogInformation("---- Reached the DELETE account method ----");

        AccountModel selectedAccountModel = _service.GetAccount(id);
        if (selectedAccountModel == null)
            return NotFound();

        _service.DeleteAccount(id);

        return Ok();
    }
}