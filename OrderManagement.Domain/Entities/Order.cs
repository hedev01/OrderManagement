using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }

        public Guid CustomerId { get; private set; }

        public OrderStatus Status { get; private set; }

        public decimal TotalPrice { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public Customer Customer { get; private set; } = null!;

        private readonly List<OrderItem> _items = new();

        public IReadOnlyCollection<OrderItem> Items =>
            _items.AsReadOnly();

        private Order()
        {
        }

        public Order(Guid customerId)
        {
            Id = Guid.NewGuid();

            CustomerId = customerId;

            Status = OrderStatus.Pending;

            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(OrderItem item)
        {
            _items.Add(item);

            TotalPrice += item.TotalPrice;
        }
    }
}
