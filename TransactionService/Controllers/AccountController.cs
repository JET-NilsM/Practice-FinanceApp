using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Models;

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
        
        //validate email address. I can either use the MailAddress class purely to check if it is a valid format
        //or replace the EmailAddress property in the Account class with the class instance
        //since there are some variables in the class that might need to be accessed. still null.
        // try
        // {
        //     MailAddress mailAddress = new MailAddress(account.Email);
        // }
        // catch (FormatException)
        // {
        //     return BadRequest("Invalid email address format.");
        // }
        
        //validate phone number
        
        //password validation
        
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

    // [HttpPut( "{id:int}")]
    // public async Task<IActionResult> UpdateAccount(int id, Account account)
    // {
    //     Account selectedAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == account.Id);
    //     if (selectedAccount == null)
    //         return NotFound($"Account with ID: {account.Id} not found.");
    //
    //     selectedAccount = account;
    //     
    //     return Ok("Full account updated successfully.");
    // }
    //
    // [HttpPatch("{accountId:int}")]
    // public async Task<IActionResult> UpdateEmailAddress(int accountId, string emailAddress)
    // {
    //     Account selectedAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == accountId);
    //     if (selectedAccount == null)
    //         return NotFound($"Account with ID: {accountId} not found.");
    //     
    //     //email address validation before assigning
    //     
    //     selectedAccount.Email = emailAddress;
    //
    //     return Ok("Email address updated successfully.");
    // }

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