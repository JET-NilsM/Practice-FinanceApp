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
        
        if(!Utilities.Email.IsValidEmail(account.Email))
            return BadRequest($"Invalid email address: {account.Email}");
        
        //TODO validate phone number
        
        //TODO password validation
        
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
        
        if(!Utilities.Email.IsValidEmail(newAccountData.Email))
            return BadRequest($"Invalid email address: {newAccountData.Email}");
        
        int selectedAccountIndex = AccountsModel.Accounts.IndexOf(selectedAccount);

        AccountsModel.Accounts[selectedAccountIndex] = newAccountData;
        
        return Ok($"Full account updated successfully.");
    }
    
    //I am using the [FromBody] attribute here because otherwise it is expected as a parameter in Insomnia rather than in the body of the request. And to stay consistent 
    //I am using the body as input here as well.
    [HttpPatch("{givenID:int}")]
    public async Task<IActionResult> UpdateEmailAddress(int givenID, [FromBody]string newEmail)
    {
        Account selectedAccount = AccountsModel.Accounts.FirstOrDefault(a => a.Id == givenID);
        if (selectedAccount == null)
            return NotFound($"Account with ID: {givenID} not found.");
        
        if(!Utilities.Email.IsValidEmail(newEmail))
            return BadRequest($"Invalid email address: {newEmail}");
        
        selectedAccount.Email = newEmail;
    
        return Ok($"Email address updated to {newEmail} successfully.");
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