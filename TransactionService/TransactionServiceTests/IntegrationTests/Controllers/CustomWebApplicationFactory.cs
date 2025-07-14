using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TransactionService.Data;

namespace TransactionServiceTests.Integration_Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            var config = context.Configuration;
            var connectionString = config.GetConnectionString("TestDb");
            
            services.RemoveAll<DbContextOptions<FinanceContext>>();
            services.AddDbContext<FinanceContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        });
        
        builder.UseEnvironment("Development");
    }
}