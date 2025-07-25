using Microsoft.AspNetCore.Mvc;
using TransactionService.Models;
using TransactionService.Repositories;
using TransactionService.Utilities;

namespace TransactionService.Controllers;

[ApiController] //or [FromBody] if it should only be used for specific methods
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
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
        _logger.LogInformation("---- Reached the PUT account method ----");

        if (incomingData == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(); 

        Account existingAccount = _repo.GetAccount(incomingData.ID);
        if (existingAccount != null)
            return BadRequest();

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
            return NotFound();

        return Ok(selectedAccount);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        _logger.LogInformation("---- Reached the GET accounts method ----");

        List<Account> accounts = _repo.GetAccounts();
        if (accounts == null)
            return NotFound();

        if (accounts.Count == 0)
            return NotFound();

        return Ok(accounts);
    }

    [HttpPut("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Account newAccountData)
    {
        _logger.LogInformation("---- Reached the PUT account method ----");

        Account selectedAccount = _repo.GetAccount(givenID);
        if (selectedAccount == null)
            return NotFound();

        _repo.UpdateAccount(givenID, newAccountData);

        return Ok(newAccountData);
    }

    [HttpPatch("{givenID:int}")]
    public async Task<IActionResult> UpdateAccount(int givenID, Dictionary<string, object> newData)
    {
        _logger.LogInformation("---- Reached the PATCH account method ----");

        Account updatedAccount = _repo.GetAccount(givenID);
        if (updatedAccount == null)
            return NotFound();

        if (!newData.Any())
            return BadRequest();

        string allDictionaryContents = "";

        foreach (var dataEntry in newData)
        {
            object? convertedValue = AccountHelper.ConvertToProperty(dataEntry, updatedAccount);

            if (convertedValue == null)
                return BadRequest();

            //Just for debugging to check if the dictionary is being processed correctly from Insomnia > controller
            allDictionaryContents += $"{dataEntry.Key}: {convertedValue}\n";
        }

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        _logger.LogInformation("---- Reached the DELETE account method ----");

        Account selectedAccount = _repo.GetAccount(id);
        if (selectedAccount == null)
            return NotFound();

        _repo.DeleteAccount(id);

        return Ok();
    }
}