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
    public async void CreateAccount_ShouldReturnBadRequest_WhenAccountAlreadyExists()
    {
        // Arrange
        var accountController = new AccountController();
        var existingAccount = new Account
        {
            Id = 1,
            FullName = "unit test user",
            Email = "unitTest@gmail.com",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 100.0f,
            AccountType = AccountType.Student,
        };
        
        // Act
        var result = await accountController.CreateAccount(existingAccount);
        
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Account with ID: 1 already exists.", badRequestResult.Value);
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
    public async void GetExistingAccounts_ShouldReturnOk()
    {
        // Arrange
        var accountController = new AccountController();
        
        // Act
        var result = await accountController.GetAccounts();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(okResult.Value.GetType(), typeof(List<Account>));
    }

    [Fact]
    public async void GetAccount_ShouldReturnBadRequest_WhenAccountIsNull()
    {
        // Arrange
        var accountController = new AccountController();
        
        // Act
        var result = await accountController.GetAccount(-1);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Account with ID: -1 not found.", notFoundResult.Value);
    }
    
    [Fact]
    public async void GetAccount_ShouldReturnOk_WhenAccountExists()
    {
        // Arrange
        var accountController = new AccountController();
        
        // Act
        var result = await accountController.GetAccount(1);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Account>(okResult.Value);
    }

    [Fact]
    public async Task NewAccount_ShouldReturnBadRequest_WhenEmailAddressIsInvalid()
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
    public async Task NewAccount_ShouldReturnBadRequest_WhenUsingNonWhitelistedEmailDomain()
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
    public async Task UpdateAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountController = new AccountController();
        var newData = new Account()
        {
            Id = 1,
            FullName = "Updated User",
            Email = "test@gmail.com",
            Password = "newPassword123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 500f,
            AccountType = AccountType.Student,
        };
        
        // Act
        var result = await accountController.UpdateAccount(-1, newData);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Account with ID: -1 not found.", notFoundResult.Value);
    }
    
    [Fact]
    public async Task UpdateAccount_ShouldReturnBadRequest_WhenUsingEmptyData()
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
    public async Task UpdateAccount_ShouldReturnOk_WhenAccountIsUpdated()
    {
        // Arrange
        var accountController = new AccountController();
        Dictionary<string, object> newData = new Dictionary<string, object>
        {
            { "Email", "unitTest@gmail.com" }
        };
        
        // Act
        var result = await accountController.UpdateAccount(1, newData);
        
        // Assert
        var updatedResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"Account with ID: 1 updated successfully. New data: Email: unitTest@gmail.com", updatedResult.Value.ToString().Trim());
    }

    [Fact]
    public async Task DeleteAccount_ShouldReturnOk_WhenAccountDeleted()
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
    
    [Fact]
    public async Task DeleteAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountController = new AccountController();
        
        // Act
        var result = await accountController.DeleteAccount(-1);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Account with ID: -1 not found.", notFoundResult.Value);
    }
}