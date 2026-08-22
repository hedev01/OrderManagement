using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Username { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;

        public UserRole Role { get; private set; }


        private User()
        {
        }

        public User(
            string username,
            string passwordHash,
            UserRole role)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}
