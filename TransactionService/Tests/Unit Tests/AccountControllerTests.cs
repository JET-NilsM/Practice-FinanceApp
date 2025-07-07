using Microsoft.AspNetCore.Mvc;
using TransactionService;
using TransactionService.Controllers;
using TransactionService.Models;
using Xunit;

namespace TransactionService.Tests.Unit_Tests;

public class AccountControllerTests
{
    [Fact]
    public async void CreateAccount_ShouldReturnOk_WhenValidAccount()
    {
        // Arrange
        var accountController = new AccountController();
        var newAccount = new Account
        {
            Id = 123,
            FullName = "unit test user",
            Email = "unitTest@gmail.com",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 1010.0f,
            AccountType = AccountType.Student,
        };

        // Act
        var result = await accountController.CreateAccount(newAccount);
        var isOkResult = Assert.IsType<OkObjectResult>(result);

        // Assert
        Assert.Equal("Account created successfully.", isOkResult.Value);
        Assert.Contains(newAccount, AccountsModel.Accounts);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnBadRequest_WhenAccountIsNull()
    {
        // Arrange
        var accountController = new AccountController();

        // Act
        var result = await accountController.CreateAccount(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Account data is null.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnBadRequest_WhenEmailAddressIsInvalid()
    {
        // Arrange   
        var accountController = new AccountController();
        var newAccount = new Account
        {
            Id = 124,
            FullName = "unit test user",
            Email = "invalidEmailAddress",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 200f,
            AccountType = AccountType.Student,
        };
        
        // Act
        var result = await accountController.CreateAccount(newAccount);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"Email address must contain '@' and '.' characters.", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnBadRequest_WhenUsingNonWhitelistedEmailDomain()
    {
        // Arrange   
        var accountController = new AccountController();
        var newAccount = new Account
        {
            Id = 125,
            FullName = "unit test user",
            Email = "testUser@notWhitelisted.com",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 300f,
            AccountType = AccountType.Student,
        };

        // Act
        var result = await accountController.CreateAccount(newAccount);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Domain is not whitelisted.", badRequestResult.Value);
    }
}