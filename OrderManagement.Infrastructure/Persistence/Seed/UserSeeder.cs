using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Persistence.Seed
{
    public static class UserSeeder
    {
       
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var passwordHash =
                BCrypt.Net.BCrypt.HashPassword("Admin123");

            var admin = new User(
                "admin",
                passwordHash,
                UserRole.Admin);

            await context.Users.AddAsync(admin);

            await context.SaveChangesAsync();
        }
    }
}
