using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            await _context.Orders.AddAsync(
                order,
                cancellationToken);
        }

        public async Task AddRangeAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken)
        {
            await _context.Orders.AddRangeAsync(
                orders,
                cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<(IReadOnlyList<Order> Items, int TotalCount)>
            SearchAsync(
                Guid? customerId,
                OrderStatus? status,
                DateTime? fromDate,
                DateTime? toDate,
                int page,
                int pageSize,
                CancellationToken cancellationToken)
        {
            IQueryable<Order> query =
                _context.Orders
                    .AsNoTracking();

            if (customerId.HasValue)
            {
                query = query.Where(
                    x => x.CustomerId == customerId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(
                    x => x.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(
                    x => x.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(
                    x => x.CreatedAt <= toDate.Value);
            }

            var totalCount =
                await query.CountAsync(
                    cancellationToken);

            var items =
                await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Order?> GetForStatusChangeAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Inventory)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public Task DeleteAsync(
            Order order,
            CancellationToken cancellationToken)
        {
            _context.Orders.Remove(order);

            return Task.CompletedTask;
        }

        public async Task<Order?> GetForDeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }
    }
}
