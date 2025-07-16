using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TransactionService.Models;

namespace TransactionServiceTests.Integration_Tests;

/// <summary>
/// https://medium.com/@rjrocks299/a-practical-guide-to-database-integration-testing-with-net-62f0cf444581
///
/// [Theory] is used to run tests with parameters and different data sets, for both unit and integration tests.
/// [Fact] is used for simple tests that do not require parameters.
/// </summary>
public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client;
    
    public AccountControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async void CreateAccunt_ReturnsCreated()
    {
        Account mewAccount = new Account
        {
            FullName = "John Doe",
            Email = "test@gmail.com",
            Password = "password123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 100.0f,
            AccountType = AccountType.Student
        };
    }
    
    
    //Duplicate ID should return badrequest

    //Invalid data (invalid email) should return badrequest


    [Fact]
    public async Task GetAccounts_ReturnsOk()
    {
        var response =  await _client.GetAsync("/api/account");
        
        response.EnsureSuccessStatusCode();
        
        var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
        
        Assert.NotNull(accounts);
        Assert.Contains(accounts, a => a.FullName == "Nils Meijer");
    }
    
    
    
    //get account by id should return ok with account data

    //get account by id should return notfound if account does not exist


    //get all accounts should return ok with list of accounts

    //get all accounts should return empty list if no accounts exist


    //put update account should return ok if account exists and is updated successfully

    //put update account should return notfound if account does not exist


    //delete account should return ok if account exists and is deleted successfully

    //delete account should return notfound if account does not exist


    //patch account should return ok if account exists and is patched successfully

    //patch account should return notfound if account does not exist

    //patch account should return badrequest if patch data is invalid
}