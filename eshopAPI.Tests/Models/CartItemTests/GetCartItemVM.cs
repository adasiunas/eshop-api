using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace eshopAPI.Tests.Models.CartItemTests
{
    public class GetCartItemVM
    {
        [Fact]
        public void WithOneAttributeAndNoPicture()
        {
            Item item = new ItemBuilder().AddAttributes(1).Build();
            CartItem cartItem = CreateCartItem(item);

            CartItemVM vm = cartItem.GetCartItemVM();

            Assert.Null(vm.MainPicture);
            Assert.Equal(1, vm.Attributes.Count);
            AssertModelEqualsVM(cartItem, vm);
        }
        
        [Fact]
        public void WithMoreAttributesAndSinglePicture()
        {
            Item item = new ItemBuilder().AddAttributes(3).AddPictures(1).Build();
            CartItem cartItem = CreateCartItem(item);

            CartItemVM vm = cartItem.GetCartItemVM();

            Assert.Equal(cartItem.Item.Pictures.First().URL, vm.MainPicture);
            Assert.Equal(2, vm.Attributes.Count);
            AssertModelEqualsVM(cartItem, vm);
        }

        CartItem CreateCartItem(Item item)
        {
            return new CartItem
            {
                ID = 1,
                Count = 1,
                Item = item,
                ItemID = item.ID
            };
        }

        void AssertModelEqualsVM(CartItem item, CartItemVM vm)
        {
            Assert.Equal(item.ID, vm.ID);
            Assert.Equal(item.ItemID, vm.ItemID);
            Assert.Equal(item.Item.SKU, vm.SKU);
            Assert.Equal(item.Item.Name, vm.Name);
            Assert.Equal(item.Item.Price, vm.Price);
            Assert.Equal(item.Count, vm.Count);
            Assert.Equal(item.Item.CategoryID, vm.CategoryID);
            Assert.Equal(item.Item.SubCategoryID, vm.SubCategoryID);
            Assert.Equal(0, vm.Discount);
        }
    }
}
