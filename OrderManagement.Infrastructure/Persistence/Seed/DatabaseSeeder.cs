using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Persistence.Seed
{
    public sealed class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            await UserSeeder.SeedAsync(_context);

            await CustomerSeeder.SeedAsync(_context);

            await ProductSeeder.SeedAsync(_context);
        }
    }
}
