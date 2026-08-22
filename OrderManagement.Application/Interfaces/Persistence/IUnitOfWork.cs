using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(
            CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(
            CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(
            CancellationToken cancellationToken = default);
    }
}
