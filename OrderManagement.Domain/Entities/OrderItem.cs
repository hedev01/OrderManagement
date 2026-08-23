using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice =>
            UnitPrice * Quantity;

        public Order Order { get; private set; } = null!;

        public Product Product { get; private set; } = null!;

        private OrderItem()
        {
        }

        public OrderItem(
            Guid productId,
            int quantity,
            decimal unitPrice)
        {
            Id = Guid.NewGuid();

            ProductId = productId;

            Quantity = quantity;

            UnitPrice = unitPrice;
        }
    }
}
