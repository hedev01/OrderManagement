using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class Inventory
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        public Product Product { get; private set; } = null!;

        private Inventory()
        {
        }

        public Inventory(
            Guid productId,
            int quantity)
        {
            Id = Guid.NewGuid();

            ProductId = productId;
            Quantity = quantity;
        }
        public void Decrease(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            if (Quantity < quantity)
            {
                throw new InvalidOperationException(
                    "Insufficient inventory.");
            }

            Quantity -= quantity;
        }
    }
}
