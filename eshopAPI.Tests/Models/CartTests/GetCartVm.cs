using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Models
{
    public class GetCartVM
    {
        [Fact]
        public void Success()
        {
            Cart cart = CreateCart();
            CartVM vm = cart.GetCartVM();

            Assert.Equal(cart.ID, vm.ID);
            Assert.Equal(cart.Items.Count, vm.Items.Count);
        }

        Cart CreateCart()
        {
            Cart cart = new Cart
            {
                ID = 1,
                Items = new List<CartItem>()
            };
            Item item1 = new ItemBuilder().AddAttributes(4).AddPictures(1).Build();
            Item item2 = new ItemBuilder().AddAttributes(3).AddPictures(2).Build();
            CartItem cartItem1 = CreateCartItem(item1, 2);
            CartItem cartItem2 = CreateCartItem(item2, 2);

            cart.Items.Add(cartItem1);
            cart.Items.Add(cartItem2);

            return cart;
        }

        CartItem CreateCartItem(Item item, int count)
        {
            return new CartItem
            {
                ID = 1,
                Item = item,
                Count = count,
                ItemID = item.ID
            };
        }
    }
}
