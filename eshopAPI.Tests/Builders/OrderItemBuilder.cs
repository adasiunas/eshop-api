using eshopAPI.Models;
using eshopAPI.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class OrderItemBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public int Count { get; set; } = 2;
        public int Price { get; set; } = 50;

        static int _id = 1;

        OrderItem _orderItem;

        public OrderItemBuilder()
        {
            _orderItem = WithDefaultValues();
        }

        public OrderItemBuilder New()
        {
            _orderItem = WithDefaultValues();
            return this;
        }

        OrderItem WithDefaultValues()
        {
            _orderItem = new OrderItem
            {
                Count = Count,
                Price = Price
            };
            return _orderItem;
        }

        public OrderItemBuilder Random()
        {
            Random rnd = new Random();
            _orderItem = new OrderItem
            {
                Count = rnd.Next(1, 10),
                Price = rnd.Next(5, 1000),
            };

            Item item = new ItemBuilder().Random().Build();
            _orderItem.Item = item;
            _orderItem.ItemID = item.ID;

            return this;
        }

        public OrderItemBuilder SetItem(Item item)
        {
            _orderItem.Item = item;
            _orderItem.ItemID = item.ID;
            return this;
        }

        public OrderItem Build()
        {
            _orderItem.ID = ID;
            return _orderItem;
        }
    }
}
