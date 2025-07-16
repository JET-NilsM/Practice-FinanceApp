using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TransactionService.Data;
using TransactionService.Models;

namespace TransactionServiceTests.Integration_Tests;

/// <summary>
/// The credentials in this class or anywhere in this project are NOT real credentials, their only purpose is to serve as test data.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            var config = context.Configuration;
            var connectionString = config.GetConnectionString("TestDatabase");
            
            services.RemoveAll<DbContextOptions<FinanceContext>>();
            services.AddDbContext<FinanceContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<FinanceContext>();

            db.Database.EnsureCreated();
            db.Accounts.RemoveRange(db.Accounts);

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
                AccountBalance = 500.0f,
                AccountType = AccountType.Student
            },
            new Account()
            {
                FullName = "Henk Jansen",
                Email = "test@gmail.com",
                Password = "testpassword123",
                PhoneNumber = "0612345678",
                AccountBalance = 500.0f,
                AccountType = AccountType.Student
            }
        });

        context.SaveChanges();
    }
    
}