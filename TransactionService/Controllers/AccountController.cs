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
    private IAccountRepository _repo;

    public AccountController(IAccountRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(Account account)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest("Invalid account data provided.");
        }

        //Name "empty" check is not necessary since the FullName property is required in the Account model,
        //It could be checked whether the name is valid but that is probably not necessary for now
        if (account == null)
            return BadRequest("Account data is null.");

        //Check if ID already exists
        Account existingAccount = _repo.GetAccount(account.ID);
        if (existingAccount != null)
            return BadRequest($"Account with ID: {account.ID} already exists.");

        Account newAccount = new Account()
        {
            ID = account.ID,
            FullName = account.FullName,
            Email = account.Email,
            Password = account.Password,
            PhoneNumber = account.PhoneNumber,
            Data = new List<AccountData>()
            {
                new AccountData()
                {
                    Account = account,
                    Balance = account.Data?.FirstOrDefault()?.Balance ?? 0.0f, // Default to 0 if no balance is provided
                    Type = account.Data?.FirstOrDefault()?.Type ?? AccountType.Student // Default to Student if no type is provided
                }
            }
        };

        _repo.AddAccount(newAccount);

        return Created($"/api/account/{newAccount.ID}", newAccount);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        Account selectedAccount = _repo.GetAccount(id);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {id} not found.");

        return Ok(selectedAccount);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
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
        Account selectedAccount = _repo.GetAccount(givenID);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {givenID} not found.");

        _repo.UpdateAccount(givenID, newAccountData);

        return Ok($"Full account updated successfully.");
    }

    [HttpPatch("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Dictionary<string, object> newData)
    {
        Account selectedAccount = _repo.GetAccount(givenID);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {givenID} not found.");

        if (!newData.Any())
            return BadRequest("No data provided for update.");

        string allDictionaryContents = "";

        foreach (var dataEntry in newData)
        {
            object? convertedValue = AccountHelper.ConvertToProperty(dataEntry, selectedAccount);

            if (convertedValue == null)
                return BadRequest($"Invalid data for property: {dataEntry.Key}");

            //Just for debugging to check if the dictionary is being processed correctly from Insomnia > controller
            allDictionaryContents += $"{dataEntry.Key}: {convertedValue}\n";
        }

        return Ok($"Account with ID: {givenID} updated successfully. New data: {allDictionaryContents}");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        Account selectedAccount = _repo.GetAccount(id);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {id} not found.");

        _repo.DeleteAccount(id);

        return Ok("Account deleted successfully.");
    }
}