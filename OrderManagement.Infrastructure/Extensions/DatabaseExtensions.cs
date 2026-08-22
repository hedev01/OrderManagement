using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.Persistence.Seed;

namespace OrderManagement.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task SeedDatabaseAsync(
            this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var seeder =
                scope.ServiceProvider
                    .GetRequiredService<IDatabaseSeeder>();

            await seeder.SeedAsync();
        }
    }
}
