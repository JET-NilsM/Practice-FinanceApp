using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TransactionService.Data;
using TransactionService.Models;
using Xunit.Extensions.Logging;

namespace TransactionServiceTests.Integration_Tests;

/// <summary>
/// The credentials in this class or anywhere in this project are NOT real credentials, their only purpose is to serve as test data.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
        
        builder.ConfigureServices((context, services) =>
        {
            var config = context.Configuration;
            var connectionString = config.GetConnectionString("DefaultConnection");
            
            services.RemoveAll<DbContextOptions<FinanceContext>>();
            services.AddDbContext<FinanceContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<FinanceContext>();

            db.Database.EnsureCreated();
            db.Accounts.RemoveRange(db.Accounts);
            db.Database.ExecuteSqlRaw("ALTER SEQUENCE \"Accounts_ID_seq\" RESTART WITH 1;");
            db.Database.ExecuteSqlRaw("ALTER SEQUENCE \"AccountData_ID_seq\" RESTART WITH 1;");

            SeedDatabase(db);
        });
        
        builder.UseEnvironment("Development");
    }

    private void SeedDatabase(FinanceContext context)
    {
        context.Accounts.AddRange(new List<Account>()
        {
            new Account
            {
                FullName = "Nils Meijer",
                Email = "test@gmail.com",
                Password = "testpassword123",
                PhoneNumber = "0612345678",
                Data = new List<AccountData>()
                {
                    new AccountData()
                    {
                        Balance = 100.0f,
                        Type = AccountType.Student
                    }
                }
            },
            new Account()
            {
                FullName = "Henk Jansen",
                Email = "test@gmail.com",
                Password = "testpassword123",
                PhoneNumber = "0612345678",
                Data = new List<AccountData>()
                {
                    new AccountData()
                    {
                        Balance = 200.0f,
                        Type = AccountType.Youth
                    }
                }
            }
        });

        context.SaveChanges();
    }
    
}