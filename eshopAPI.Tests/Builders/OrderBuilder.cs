using eshopAPI.Models;
using eshopAPI.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class OrderBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public OrderStatus Status { get; set; } = OrderStatus.Accepted;
        public string DeliveryAddress { get; set; } = "HouseOfRisingSun";

        static int _id = 1;

        Order _order;

        public OrderBuilder()
        {
            _order = WithDefaultValues();
        }

        public OrderBuilder New()
        {
            _order = WithDefaultValues();
            return this;
        }

        Order WithDefaultValues()
        {
            _order = new Order
            {
                DeliveryAddress = DeliveryAddress,
                Status = OrderStatus.Accepted,
                CreateDate = DateTime.UtcNow,
                User = new ShopUserBuilder().Build(),
                Items = new List<OrderItem>()
            };
            return _order;
        }

        public OrderBuilder Random()
        {
            Random rnd = new Random();
            _order = new Order
            {
                DeliveryAddress = rnd.RandomString(10),
                CreateDate = DateTime.UtcNow,
                User = new ShopUserBuilder().Build(),
                Items = new List<OrderItem>()
            };
            int itemsCnt = rnd.Next(1, 5);
            //AddItems(itemsCnt);

            return this;
        }

        public OrderBuilder AddItems(int count)
        {
            List<OrderItem> items = new List<OrderItem>();
            for (int i = 0; i < count; i++)
            {
                OrderItem item = new OrderItemBuilder().Random().Build();
                items.Add(item);
            }
            _order.Items = items;
            return this;
        }

        public OrderBuilder SetUser(ShopUser user)
        {
            _order.User = user;
            return this;
        }

        public OrderBuilder SetItems(List<OrderItem> items)
        {
            _order.Items = items;
            return this;
        }

        public Order Build()
        {
            _order.ID = ID;
            _order.OrderNumber = (int)_order.ID + 1000000;
            return _order;
        }
    }
}
