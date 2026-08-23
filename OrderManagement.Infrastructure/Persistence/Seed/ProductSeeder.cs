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

    public static class ProductSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            await using var transaction =
                await context.Database.BeginTransactionAsync();

            try
            {
                var products = new List<Product>();

                for (var i = 1; i <= 200; i++)
                {
                    var product = new Product(
                        name: $"Product {i}",
                        description: $"Description for product {i}",
                        price: 100000 + (i * 5000));

                    products.Add(product);
                }

                await context.Products.AddRangeAsync(products);

                await context.SaveChangesAsync();

                var inventories = products
                    .Select(product =>
                        new Inventory(
                            product.Id,
                            Random.Shared.Next(10, 101)))
                    .ToList();

                await context.Inventories.AddRangeAsync(
                    inventories);

                await context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
