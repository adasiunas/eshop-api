using eshopAPI.Models;
using eshopAPI.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class CartItemBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get
            {
                return _id++;
            }
        }
        public int count { get; set; } = 1;

        static int _id = 1;

        CartItem _cartItem;

        public CartItemBuilder()
        {
            _cartItem = WithDefaultValues();
        }

        CartItem WithDefaultValues()
        {
            Item _item = new ItemBuilder().Build();
            _cartItem = new CartItem
            {
                Count = count,
                Item = _item,
                ItemID = _item.ID
            };
            return _cartItem;
        }

        public CartItemBuilder Random()
        {
            Random rnd = new Random();
            Item _item = new ItemBuilder().Random().Build();
            _cartItem = new CartItem
            {
                Count = rnd.Next(1, 100),
                Item = _item,
                ItemID = _item.ID
            };
            return this;
        }

        public CartItemBuilder SetItem(Item item)
        {
            _cartItem.Item = item;
            _cartItem.ItemID = item.ID;
            return this;
        }

        public CartItem Build()
        {
            _cartItem.ID = ID;
            return _cartItem;
        }
    }
}
