using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public string PhoneNumber { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }


        private Customer()
        {
        }

        public Customer(
            string firstName,
            string lastName,
            string email,
            string phoneNumber)
        {
            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;

            CreatedAt = DateTime.UtcNow;
        }
    }
}
