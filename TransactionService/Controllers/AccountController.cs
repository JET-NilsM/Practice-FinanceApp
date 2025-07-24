using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Data;
using TransactionService.Models;
using TransactionService.Repositories;
using TransactionService.Utilities;
using Xunit.Sdk;

namespace TransactionService.Controllers;

[ApiController] //or [FromBody] if it should only be used for specific methods
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private AccountsModel _accountsModel;
    private IAccountRepository _repo;
    private readonly ILogger _logger;

    public AccountController(IAccountRepository repo, ILogger<AccountController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(Account incomingData)
    {
         if (!ModelState.IsValid)
         {
             var errors = ModelState.Values.SelectMany(v => v.Errors)
                 .Select(e => e.ErrorMessage)
                 .ToList();
             
             _logger.LogError("Model state is invalid. Errors: {Errors}", string.Join(", ", errors));
             
             return BadRequest("Invalid account data provided.");
         }
        
         _logger.LogInformation("---- Reached the PUT account method ----");
        
         //Name "empty" check is not necessary since the FullName property is required in the Account model,
         //It could be checked whether the name is valid but that is probably not necessary for now
         if (incomingData == null)
             return BadRequest("Account data is null.");

         Account existingAccount = _repo.GetAccount(incomingData.ID);
         if (existingAccount != null)
             return BadRequest($"Account with ID: {incomingData.ID} already exists.");

         Account newAccount = new Account()
         {
             ID = incomingData.ID,
             FullName = incomingData.FullName,
             Email = incomingData.Email,
             Password = incomingData.Password,
             PhoneNumber = incomingData.PhoneNumber
         };

        _repo.AddAccount(newAccount);

        return Created($"/api/account/{newAccount.ID}", newAccount);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        _logger.LogInformation("---- Reached the GET account method ----");

        Account selectedAccount = _repo.GetAccount(id);
        if (selectedAccount == null)
            return NotFound(id);

        return Ok(selectedAccount);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        _logger.LogInformation("---- Reached the GET accounts method ----");

        List<Account> accounts = _repo.GetAccounts();
        if (accounts == null)
            return NotFound("Accounts collection does not exist");

        if (accounts.Count == 0)
            return NotFound("Accounts collection is empty.");

        return Ok(accounts);
    }

    [HttpPut("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Account newAccountData)
    {
        _logger.LogInformation("---- Reached the PUT account method ----");

        Account selectedAccount = _repo.GetAccount(givenID);
        if (selectedAccount == null)
            return NotFound(givenID);

        _repo.UpdateAccount(givenID, newAccountData);

        return Ok(newAccountData);
    }

    [HttpPatch("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Dictionary<string, object> newData)
    {
        _logger.LogInformation("---- Reached the PATCH account method ----");

        Account updatedAccount = _repo.GetAccount(givenID);
        if (updatedAccount == null)
            return NotFound($"Account with ID: {givenID} not found.");

        if (!newData.Any())
            return BadRequest("No data provided for update.");

        string allDictionaryContents = "";

        foreach (var dataEntry in newData)
        {
            object? convertedValue = AccountHelper.ConvertToProperty(dataEntry, updatedAccount);

            if (convertedValue == null)
                return BadRequest($"Invalid data for property: {dataEntry.Key}");

            //Just for debugging to check if the dictionary is being processed correctly from Insomnia > controller
            allDictionaryContents += $"{dataEntry.Key}: {convertedValue}\n";
        }

        return Ok(updatedAccount);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        _logger.LogInformation("---- Reached the DELETE account method ----");

        Account selectedAccount = _repo.GetAccount(id);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {id} not found.");

        _repo.DeleteAccount(id);

        return Ok();
    }
}