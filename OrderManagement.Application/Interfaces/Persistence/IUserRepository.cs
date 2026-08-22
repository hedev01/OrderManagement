using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            User user,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default);
    }
}
