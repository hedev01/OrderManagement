using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Persistence.Seed
{
    public static class CustomerSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            if (await context.Customers.AnyAsync())
                return;

            var customers = Enumerable
                .Range(1, 50)
                .Select(i =>
                    new Customer(
                        firstName: $"Customer{i}",
                        lastName: $"Test{i}",
                        email: $"customer{i}@example.com",
                        phoneNumber: $"091200000{i:D2}"
                    ))
                .ToList();

            await context.Customers.AddRangeAsync(customers);

            await context.SaveChangesAsync();
        }
    }
}
