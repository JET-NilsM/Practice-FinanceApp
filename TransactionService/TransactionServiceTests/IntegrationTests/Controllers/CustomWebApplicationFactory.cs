using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TransactionService.Data;

namespace TransactionServiceTests.Integration_Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString = "Server=localhost;Database=TransactionServiceTestDB;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DbContext>>();

            services.AddDbContext<FinanceContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });
        });
        
        builder.UseEnvironment("Development");
    }
}