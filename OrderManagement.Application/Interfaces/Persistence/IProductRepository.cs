using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Persistence
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken);
    }
}
