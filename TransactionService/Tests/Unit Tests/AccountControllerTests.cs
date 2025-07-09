using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TransactionService;
using TransactionService.Controllers;
using TransactionService.Models;
using Xunit;
using Xunit.Abstractions;

namespace TransactionService.Tests.Unit_Tests;

public class AccountControllerTests
{
    private readonly ITestOutputHelper output;

    public AccountControllerTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    [Fact]
    public async void CreateAccount_ShouldReturnOk_WhenValidAccount()
    {
        // Arrange
        var accountController = new AccountController();
        var newAccount = new Account
        {
            Id = 123,
            FullName = "unit test user",
            Email = "unitTest",
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
        var newAccount = new Account
        {
            Id = 124,
            FullName = "unit test user",
            Email = "email@invalidDomain.com",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 200f,
            AccountType = AccountType.Student,
        };

        //Act
        var validationContext = new ValidationContext(newAccount);
        var validationResults = new List<ValidationResult>();
        var isValidAccount = Validator.TryValidateObject(newAccount, validationContext, validationResults, true);
        
        //Assert
        Assert.False(isValidAccount);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnBadRequest_WhenUsingNonWhitelistedEmailDomain()
    {
        // Arrange   
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
        var validationContext = new ValidationContext(newAccount);
        var validationResults = new List<ValidationResult>();
        var isValidAccount = Validator.TryValidateObject(newAccount, validationContext, validationResults, true);
        
        //Assert
        Assert.False(isValidAccount);
    }
    
    [Fact]
    public async Task DeleteAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountController = new AccountController();
        Account newAccount = new Account
        {
            Id = 126,
            FullName = "unit test user",
            Email = "test@gmail.com",
            PhoneNumber = "+31 6 12345678",
            Password = "testPassword123",
            AccountBalance = 500f,
            AccountType = AccountType.Student,
        };

        accountController.CreateAccount(newAccount);
        
        // Act
        var result = await accountController.DeleteAccount(newAccount.Id);
        
        // Assert
        var notFoundResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"Account deleted successfully.", notFoundResult.Value);
    }
}