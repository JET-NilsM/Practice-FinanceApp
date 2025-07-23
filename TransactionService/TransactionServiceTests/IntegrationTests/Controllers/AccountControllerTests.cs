using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
    public async Task CreateAccount_ReturnsCreated()
    {
        var newAccount = new 
        {
            FullName = "John Doe",
            Email = "test@gmail.com",
            Password = "password123",
            PhoneNumber = "+31 6 12345678",
            Data = new List<AccountData>()
            {
                new AccountData()
                {
                    Balance = 100.0f,
                    Type = 0
                }
            }
        };
        
        
        var stringContent = new StringContent(JsonSerializer.Serialize(newAccount), Encoding.UTF8, "application/json");
        
        Console.WriteLine(stringContent);
        
        HttpResponseMessage response = await _client.PostAsync("/api/account", stringContent);
        
        // var returnedAccount = await response.Content.ReadFromJsonAsync<Account>();

        var res = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine(res);
        
        // Assert.NotNull(returnedAccount);
        // Assert.Equal(newAccount.FullName, returnedAccount.FullName);
    }
    
    
    //Duplicate ID should return badrequest

    //Invalid data (invalid email) should return badrequest
    
    [Fact]
    public async Task GetAccountById_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/account/1");
        
        response.EnsureSuccessStatusCode();
        
        var account = await response.Content.ReadFromJsonAsync<Account>();
        
        Assert.NotNull(account);
        Assert.Equal("Nils Meijer", account.FullName);
    }
    
    [Fact]
    public async Task GetAccount_WhenDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/account/-1");
        
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAccounts_ReturnsOk()
    {
        var response =  await _client.GetAsync("/api/account");
        
        response.EnsureSuccessStatusCode();
        
        var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
        
        Assert.NotNull(accounts);
        Assert.Contains(accounts, a => a.FullName == "Nils Meijer");
    }

    //put update account should return ok if account exists and is updated successfully

    //put update account should return notfound if account does not exist
    [Fact]
    public async Task PutAccount_AccountDoesNotExist_ReturnNotFound()
    {
        Account newData = new Account
        {
            FullName = "Updated Name",
            Email = "test@gmail.com",
            Password = "password123",
            PhoneNumber = "+31 6 12345678",
            Data = new List<AccountData>()
            {
                new AccountData()
                {
                    Balance = 100.0f,
                    Type = AccountType.Student
                }
            }
        };
        
        var json = JsonSerializer.Serialize(newData);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response =  await _client.PutAsync("/api/account/-1", stringContent);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    //delete account should return ok if account exists and is deleted successfully
    [Fact]
    public async Task DeleteAccount_ReturnsOk()
    {
        var response =  await _client.DeleteAsync("/api/account/2");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    //delete account should return notfound if account does not exist
    [Fact]
    public async Task DeleteAccount_WhenDoesNotExist_ReturnsNotFound()
    {
        var response =  await _client.DeleteAsync("/api/account/-1");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    //patch account should return ok if account exists and is patched successfully
    [Fact]
    public async Task PatchAccount_ReturnsOk()
    {
        var patchData = new Dictionary<string, object>()
        {
            { "FullName", "Updated Name" },
        };
        
        var json = JsonSerializer.Serialize(patchData);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync("/api/account/1", stringContent);
        
        response.EnsureSuccessStatusCode();
        
        var returnedAccount = await response.Content.ReadFromJsonAsync<Account>();
        
        Assert.Equal(returnedAccount.FullName, returnedAccount.FullName);
    }

    //patch account should return notfound if account does not exist
    [Fact]
    public async Task? PatchAccount_AccountDoesNotExist_ReturnNotFound()
    {
        var patchData = new Dictionary<string, object>()
        {
            { "FullName", "Updated Name" },
        };
        
        var json = JsonSerializer.Serialize(patchData);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync("/api/account/-1", stringContent);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    //patch account should return badrequest if patch data is invalid
    [Fact]
    public async Task PatchAccount_InvalidData_ReturnsBadRequest()
    {
        var patchData = new Dictionary<string, object>()
        {
            { "InvalidProperty", "Invalid Value" },
        };
        
        var json = JsonSerializer.Serialize(patchData);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync("/api/account/1", stringContent);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}