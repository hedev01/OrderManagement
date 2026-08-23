using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    customer => customer.Id == id,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Customers
                .AsNoTracking()
                .AnyAsync(
                    customer => customer.Email == email,
                    cancellationToken);
        }

        public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            await _context.Customers.AddAsync(
                customer,
                cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
