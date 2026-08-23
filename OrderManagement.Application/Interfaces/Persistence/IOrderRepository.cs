using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Interfaces.Persistence
{
    public interface IOrderRepository
    {
        Task AddAsync(
            Order order,
            CancellationToken cancellationToken);

        Task AddRangeAsync(
            IReadOnlyCollection<Order> orders,
            CancellationToken cancellationToken);

        Task<Order?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<(IReadOnlyList<Order> Items, int TotalCount)>
            SearchAsync(
                Guid? customerId,
                OrderStatus? status,
                DateTime? fromDate,
                DateTime? toDate,
                int page,
                int pageSize,
                CancellationToken cancellationToken);

        Task<Order?> GetForStatusChangeAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task DeleteAsync(
            Order order,
            CancellationToken cancellationToken);

        Task<Order?> GetForDeleteAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
