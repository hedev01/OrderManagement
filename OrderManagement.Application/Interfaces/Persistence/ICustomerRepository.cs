using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Persistence
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);
        Task<IReadOnlyList<Customer>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken);

        Task<bool> ExistsByEmailAsync(
            string email,
            CancellationToken cancellationToken);

        Task AddAsync(
            Customer customer,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Customer>> GetAllAsync(
            CancellationToken cancellationToken);
    }
}
