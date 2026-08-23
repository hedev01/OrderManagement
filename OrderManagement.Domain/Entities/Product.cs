using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public string Description { get; private set; } = null!;

        public decimal Price { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public Inventory Inventory { get; private set; } = null!;

        private Product()
        {
        }

        public Product(
            string name,
            string description,
            decimal price)
        {
            Id = Guid.NewGuid();

            Name = name;
            Description = description;
            Price = price;

            CreatedAt = DateTime.UtcNow;
        }
    }
}
