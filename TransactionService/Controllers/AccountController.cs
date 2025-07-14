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

    public AccountController(IAccountRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(Account account)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid account data provided.");

        //Name "empty" check is not necessary since the FullName property is required in the Account model,
        //It could be checked whether the name is valid but that is probably not necessary for now
        if (account == null)
            return BadRequest("Account data is null.");

        //Check if ID already exists
        Account existingAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == account.Id);
        if (existingAccount != null)
            return BadRequest($"Account with ID: {account.Id} already exists.");

        Account newAccount = new Account()
        {
            Id = account.Id,
            FullName = account.FullName,
            Email = account.Email,
            Password = account.Password,
            PhoneNumber = account.PhoneNumber,
            AccountBalance = account.AccountBalance,
            AccountType = account.AccountType
        };

        _repo.AddAccount(newAccount);

        return Ok("Account created successfully.");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        Account selectedAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == id);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {id} not found.");

        return Ok(selectedAccount);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        if (AccountsModel.Accounts.Count == 0)
            return NotFound("No accounts found");

        return Ok(AccountsModel.Accounts);
    }

    [HttpPut("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Account newAccountData)
    {
        Account selectedAccount = AccountsModel.Accounts.Where(account => account.Id == givenID).FirstOrDefault();
        if (selectedAccount == null)
            return NotFound($"Account with ID: {givenID} not found.");

        int selectedAccountIndex = AccountsModel.Accounts.IndexOf(selectedAccount);

        AccountsModel.Accounts[selectedAccountIndex] = newAccountData;

        return Ok($"Full account updated successfully.");
    }

    [HttpPatch("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Dictionary<string, object> newData)
    {
        Account selectedAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == givenID);
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
        Account selectedAccount = AccountsModel.Accounts.Where(account => account.Id == id).FirstOrDefault();
        if (selectedAccount == null)
            return NotFound($"Account with ID: {id} not found.");

        AccountsModel.Accounts.Remove(selectedAccount);

        return Ok("Account deleted successfully.");
    }
}