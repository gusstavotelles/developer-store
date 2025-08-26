using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Ambev.DeveloperEvaluation.ORM;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ambev.DeveloperEvaluation.Integration
{
    /// <summary>
    /// Custom WebApplicationFactory that replaces the real DbContext with an InMemory provider for integration tests.
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registrations (all descriptors)
                services.RemoveAll(typeof(DbContextOptions<DefaultContext>));
                services.RemoveAll(typeof(DefaultContext));

                // Add InMemory DbContext for tests
                services.AddDbContext<DefaultContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build the service provider and ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DefaultContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }
    }
}
