using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TransactionService;
using TransactionService.Controllers;
using TransactionService.Models;
using Xunit;
using Xunit.Abstractions;
using Moq;
using TransactionService.Mapper;
using TransactionService.Repositories;
using TransactionService.Services;
using TransactionServiceTests.Integration_Tests;
using Xunit.Extensions.Logging;

namespace TransactionServiceTests.UnitTests;

public class AccountControllerTests {
    
    public AccountControllerTests()
    {
        
    }
    
    
    [Fact] 
    public async void CreateAccount_ShouldReturnOk_WhenValidAccount()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        var newAccount = new AccountModel()
        {
            ID = 123,
            FullName = "unit test user",
            Email = "unitTest@gmail.com",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
        };

        mockService.Setup(service => service.GetAccount(newAccount.ID)).Returns((AccountModel)null);

        // Act
        ValidateModel(newAccount, accountController.ModelState);
        var result = await accountController.CreateAccount(newAccount);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal((int)HttpStatusCode.Created, createdResult.StatusCode);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnBadRequest_WhenAccountIsNull()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        
        // Act
        var result = await accountController.CreateAccount(null);
 
        // Assert
        var badRequestResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void GetExistingAccounts_ShouldReturnOk()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        mockService.Setup(service => service.GetAccounts()).Returns(new List<AccountModel>
        {
            new AccountModel 
                { 
                    ID = 1, 
                    FullName = "Test", 
                    Email = "test@gmail.com", 
                    Password = "password123", 
                    PhoneNumber = "0621345637", 
                    Data = new List<int>
                    {
                        1,2
                    }
                }
        });

        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        
        // Act
        var result = await accountController.GetAccounts();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(okResult.Value.GetType(), typeof(List<AccountModel>));
    }

    [Fact] 
    public async void GetAccount_ShouldReturnBadRequest_WhenAccountDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        mockService.Setup(service => service.GetAccount(-1)).Returns((AccountModel)null);
        
        // Act
        var result = await accountController.GetAccount(-1);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
    }
    
    [Fact] 
    public async void GetAccount_ShouldReturnOk_WhenAccountExists()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        mockService.Setup(service => service.GetAccount(1)).Returns(new AccountModel
        {
            ID = 1,
            FullName = "Test User",
            Email = "test@gmail.com",
            Password = "password123",
            PhoneNumber = "+31 6 12345678",
            Data = new List<int>
            {
                1,2
            }
        });
        
        // Act
        var result = await accountController.GetAccount(1);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<AccountModel>(okResult.Value);
    }

    [Fact] 
    public async Task NewAccount_ShouldReturnBadRequest_WhenEmailAddressIsInvalid()
    {
        // Arrange   
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        var newAccount = new AccountModel
        {
            ID = 5,
            FullName = "unit test user",
            Email = "invalidAddress",
            Password = "testPassword123",
            PhoneNumber = "+31 6 12345678",
            Data = new List<int>
            {
                1,2
            }
        };

        var accountData = new List<AccountData>()
        {
            new AccountData()
            {
                Balance = 100.0f,
                Type = AccountType.Student
            }
        };


        mockService.Setup(service => service.UpdateAccount(1, newAccount));

        //Act - testing both attribute validation and CRUD operation
        ValidateModel(newAccount, accountController.ModelState);
        var result = await accountController.CreateAccount(newAccount);
        
        //Assert
        var updatedResult = Assert.IsType<BadRequestResult>(result);  
        Assert.Equal((int)HttpStatusCode.BadRequest, updatedResult.StatusCode);
        mockService.Verify(service => service.AddAccount(newAccount), Times.Never);
    }
    
    [Fact] 
    public async Task UpdateAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        var newData = new AccountModel()
        {
            FullName = "Updated User",
            Email = "test@gmail.com",
            Password = "newPassword123",
            PhoneNumber = "+31 6 12345678",
        };

        var chosenAccountID = -1;
        
        // Act
        var result = await accountController.UpdateAccount(chosenAccountID, newData);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        mockService.Verify(service => service.UpdateAccount(chosenAccountID, It.IsAny<AccountModel>()), Times.Never);
    }
    
    [Fact] 
    public async Task UpdateAccount_ShouldReturnBadRequest_WhenUsingEmptyData()
    {
        // Arrange   
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        var existingAccount = new AccountModel()
        {
            ID = 1,
            FullName = "Updated User",
            Email = "test@gmail.com",
            Password = "newPassword123",
            PhoneNumber = "+31 6 12345678",
            Data = new List<int>
            {
                1,2
            }
        };

        mockService.Setup(service => service.GetAccount(existingAccount.ID)).Returns(existingAccount);
        
        
        // Act
        var  result = await accountController.UpdateAccount(1, new Dictionary<string, object>()
        {
            
        });
        
        //Assert
        var notFoundResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, notFoundResult.StatusCode);
        mockService.Verify(service => service.UpdateAccount(1, It.IsAny<AccountModel>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAccount_ShouldReturnOk_WhenAccountIsUpdated()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        var existingAccount = new AccountModel()
        {
            ID = 1,
            FullName = "Updated User",
            Email = "test@gmail.com",
            Password = "newPassword123",
            PhoneNumber = "+31 6 12345678",
            Data = new List<int>
            {
                1,2
            }
        };
        mockService.Setup(service => service.GetAccount(existingAccount.ID)).Returns(existingAccount);
        
        Dictionary<string, object> newData = new Dictionary<string, object>
        {
            { "Email", "unitTest@gmail.com" }
        };
        
        // Act
        var result = await accountController.UpdateAccount(1, newData);
        
        // Assert
        var updatedResult = Assert.IsType<OkResult>(result);
        Assert.Equal((int)HttpStatusCode.OK, updatedResult.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_ShouldReturnOk_WhenAccountDeleted()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        var existingAccount = new AccountModel
        {
            ID = 2,
            FullName = "Test User",
            Email = "test@gmail.com",
            Password = "password123",
            PhoneNumber = "+31 6 12345678",
            Data = new List<int>{
                1,2
            }
        };
        
        mockService.Setup(service => service.GetAccount(2)).Returns(existingAccount);
        
        // Act
        var result = await accountController.DeleteAccount(2);
        
        // Assert
        var updatedResult = Assert.IsType<OkResult>(result);  
        Assert.Equal((int)HttpStatusCode.OK, updatedResult.StatusCode);
        mockService.Verify(service => service.DeleteAccount(2), Times.Once);
    }
    
    [Fact] 
    public async Task DeleteAccount_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IAccountService>();
        var mockLogger = new Mock<ILogger<AccountController>>();
        var accountController = new AccountController(mockService.Object, mockLogger.Object);
        mockService.Setup(service => service.GetAccount(-1)).Returns((AccountModel)null);
        
        // Act
        var result = await accountController.DeleteAccount(-1);
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        mockService.Verify(service => service.DeleteAccount(It.IsAny<int>()), Times.Never);
    }

    private void ValidateModel(AccountModel AccountModel, ModelStateDictionary modelState)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(AccountModel, null, null);
        Validator.TryValidateObject(AccountModel, validationContext, validationResults, true);
        foreach (var validationResult in validationResults)
        {
            modelState.AddModelError(
                validationResult.MemberNames.FirstOrDefault() ?? "Email",
                validationResult.ErrorMessage
            );
        }
    }
}