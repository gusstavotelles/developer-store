using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.Configuration;
using System.Linq;

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
                // Remove existing DbContext registrations
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var ctxDescriptor = services.SingleOrDefault(d => d.ImplementationType == typeof(DefaultContext) || d.ServiceType == typeof(DefaultContext));
                if (ctxDescriptor != null)
                {
                    services.Remove(ctxDescriptor);
                }

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
