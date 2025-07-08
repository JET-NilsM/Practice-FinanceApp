using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Models;
using TransactionService.Utilities;
using Xunit.Sdk;

namespace TransactionService.Controllers;

[ApiController] //or [FromBody] if it should only be used for specific methods
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private AccountsModel _accountsModel;

    public AccountController()
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(Account account)
    {
        //Name "empty" check is not necessary since the FullName property is required in the Account model,
        //It could be checked whether the name is valid but that is probably not necessary for now
        if(account == null)
            return BadRequest("Account data is null.");
        
        //Check if ID already exists
        Account existingAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == account.Id);
        if(existingAccount != null)
            return BadRequest($"Account with ID: {account.Id} already exists.");
        
        // var phoneNumberValidationResult = Utilities.PhoneValidation.IsValidPhoneNumber(account.PhoneNumber);
        // if(!phoneNumberValidationResult.IsValid) 
        //     return BadRequest($"{phoneNumberValidationResult.Message}");
        
        //TODO password validation and hashing, out of scope for now.
        
        AccountsModel.Accounts.Add(account);
        
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
        if(AccountsModel.Accounts.Count == 0)
            return NotFound("No accounts found");
        
        return Ok(AccountsModel.Accounts);
    }

    [HttpPut( "{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Account newAccountData)
    {
        Account selectedAccount = AccountsModel.Accounts.Where(account => account.Id == givenID).FirstOrDefault();
        if(selectedAccount == null)
            return NotFound($"Account with ID: {givenID} not found.");
        
        int selectedAccountIndex = AccountsModel.Accounts.IndexOf(selectedAccount);

        AccountsModel.Accounts[selectedAccountIndex] = newAccountData;
        
        return Ok($"Full account updated successfully.");
    }
    
    [HttpPatch("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Dictionary<string, object> newData)
    {
        // TODO: add validation for the newData dictionary to ensure it contains valid keys and values.
        
        
        Account selectedAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == givenID);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {givenID} not found.");
    
        if(!newData.Any())
            return BadRequest("No data provided for update.");

        // var accountType = typeof(Account);
        // foreach (KeyValuePair<string, object> dataEntry in newData)
        // {
        //     //Figures out what type the property is like string, int, etc.
        //     var property = accountType.GetProperty(dataEntry.Key);
        //     if (property == null)
        //         continue;
        //     
        //     var propertyValue = Convert.ChangeType(dataEntry.Value, property.PropertyType);
        //     property.SetValue(selectedAccount, propertyValue);
        // }

        string allDictionaryContents = "";
        foreach (var dataEntry in newData)
        {
            if (dataEntry.Key == "email")
            {
                selectedAccount.Email = dataEntry.Value.ToString();
            }
            allDictionaryContents += $"{dataEntry.Key}: {dataEntry.Value}\n";
        }
        
        return Ok($"Account with ID: {givenID} updated successfully. Dictionary content: {allDictionaryContents}");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        Account selectedAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == id);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {id} not found.");
        
        AccountsModel.Accounts.Remove(selectedAccount);
        
        return Ok("Account deleted successfully.");
    }
}