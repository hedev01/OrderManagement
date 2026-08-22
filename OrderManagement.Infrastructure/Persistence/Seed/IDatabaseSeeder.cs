using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Infrastructure.Persistence.Seed
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync(
            CancellationToken cancellationToken = default);
    }
}
