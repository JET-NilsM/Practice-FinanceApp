using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TransactionService;
using TransactionService.Controllers;
using TransactionService.Models;
using Xunit;
using Xunit.Abstractions;
using Moq;
using TransactionService.Repositories;

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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
        
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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
        
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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
        
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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
        
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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
        var newAccount = new Account
        {
            Id = 124,
            FullName = "unit test user",
            Email = "invalidAddress",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
            AccountBalance = 200f,
            AccountType = AccountType.Student,
        };

        //Act - testing both attribute validation and CRUD operation
        var validationResults = ValidateModel(newAccount);
        foreach (var validationResult in validationResults)
        {
            accountController.ModelState.AddModelError(
                validationResult.MemberNames.FirstOrDefault() ?? "Email",
                validationResult.ErrorMessage
            );
        }
        
        var result = await accountController.CreateAccount(newAccount);
        
        //Assert
        var updatedResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid account data provided.", updatedResult.Value);
    }

    [Fact]
    public async Task NewAccount_ShouldReturnBadRequest_WhenUsingNonWhitelistedEmailDomain()
    {
        // Arrange   
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
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
        var validationResults = ValidateModel(newAccount);
        foreach (var validationResult in validationResults)
        {
            accountController.ModelState.AddModelError(
                validationResult.MemberNames.FirstOrDefault() ?? "Email",
                validationResult.ErrorMessage
            );
        }

        var result = await accountController.CreateAccount(newAccount);     
                                                                    
        //Assert                                                            
        var updatedResult = Assert.IsType<BadRequestObjectResult>(result);  
        Assert.Equal("Invalid account data provided.", updatedResult.Value);
    }
    
    [Fact]
    public async Task UpdateAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
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
        var mockRepo = new Mock<IAccountRepository>();
        var controller = new AccountController(mockRepo.Object);
        
        // Act
        var  result = await controller.UpdateAccount(1, new Dictionary<string, object>());
        
        //Assert
        Assert.Equal("No data provided for update.", ((BadRequestObjectResult)result).Value);
    }

    [Fact]
    public async Task UpdateAccount_ShouldReturnOk_WhenAccountIsUpdated()
    {
        // Arrange
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
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
        var mockRepo = new Mock<IAccountRepository>();
        var accountController = new AccountController(mockRepo.Object);
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
        var mockRepo = new Mock<IAccountRepository>();
        mockRepo.Setup(repo => 
            repo.DeleteAccount(It.IsAny<int>()))
            .Throws(new KeyNotFoundException("Account with ID: -1 not found."));
        var accountController = new AccountController(mockRepo.Object);
        
        // Act
        var result = await accountController.DeleteAccount(-1);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Account with ID: -1 not found.", notFoundResult.Value);
    }

    private List<ValidationResult> ValidateModel(Account accountModel)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(accountModel, null, null);
        Validator.TryValidateObject(accountModel, validationContext, validationResults, true);
        return validationResults;
    }
}