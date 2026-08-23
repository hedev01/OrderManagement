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
        public void ChangeStatus(OrderStatus newStatus)
        {
            if (!IsValidStatusTransition(
                    Status,
                    newStatus))
            {
                throw new InvalidOperationException(
                    $"Cannot change order status from {Status} to {newStatus}.");
            }

            Status = newStatus;
        }
        private static bool IsValidStatusTransition(
            OrderStatus currentStatus,
            OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending =>
                    newStatus == OrderStatus.Confirmed,

                OrderStatus.Confirmed =>
                    newStatus == OrderStatus.Shipped,

                OrderStatus.Shipped =>
                    newStatus == OrderStatus.Delivered,

                OrderStatus.Delivered =>
                    false,

                _ => false
            };
        }
    }
}
