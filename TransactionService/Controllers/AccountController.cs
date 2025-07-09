using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json;
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
        if (!ModelState.IsValid)
            return BadRequest("Invalid account data provided.");
        
        //Name "empty" check is not necessary since the FullName property is required in the Account model,
        //It could be checked whether the name is valid but that is probably not necessary for now
        if(account == null)
            return BadRequest("Account data is null.");
        
        //Check if ID already exists
        Account existingAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == account.Id);
        if(existingAccount != null)
            return BadRequest($"Account with ID: {account.Id} already exists.");

        Account newAccount;
        try
        {
            newAccount = new Account()
            {
                Id = account.Id,
                FullName = account.FullName,
                Email = account.Email,
                Password = account.Password,
                PhoneNumber = account.PhoneNumber,
                AccountBalance = account.AccountBalance,
                AccountType = account.AccountType
            };
        }
        catch (Exception e)
        {
            return BadRequest("Invalid account data provided.");
        }
        
        AccountsModel.Accounts.Add(newAccount);
        
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

        string allDictionaryContents = "";
        var accountType = typeof(Account);
        
        foreach (var dataEntry in newData)
        {
            try
            {
                var property = accountType.GetProperty(dataEntry.Key);
                if (property == null || !property.CanWrite)
                {
                    continue;
                }

                object convertedValue;
                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (dataEntry.Value is JsonElement jsonElement)
                {
                    object? rawValue = jsonElement.ValueKind switch
                    {
                        JsonValueKind.String => jsonElement.GetString(),
                        JsonValueKind.Number => targetType == typeof(int),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => null
                    };
                    convertedValue = Convert.ChangeType(rawValue, targetType);
                }
                else
                {
                    convertedValue = Convert.ChangeType(dataEntry.Value, targetType);
                }
                
                Console.WriteLine($"Value: {convertedValue}");
                property.SetValue(selectedAccount, convertedValue);
            
                //Just for debugging to check if the dictionary is being processed correctly from Insomnia > controller
                allDictionaryContents += $"{dataEntry.Key}: {convertedValue}\n";
            }
            catch (FormatException)
            {
                return BadRequest($"Invalid email: {dataEntry.Value}");
            }
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