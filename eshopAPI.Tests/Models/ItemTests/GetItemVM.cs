using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Models.ItemTests
{
    public class GetItemVM
    {
        [Fact]
        public void WithAllPropertiesSet()
        {
            Item item = new ItemBuilder().Random().Build();

            ItemVM vm = item.GetItemVM();

            Assert.Equal(item.Attributes.Count, vm.Attributes.ToList().Count);
            Assert.Equal(item.Pictures.Count, vm.Pictures.ToList().Count);
            Assert.Equal(item.SubCategory.ID, vm.SubCategory.ID);
            Assert.Equal(item.SubCategory.Name, vm.SubCategory.Name);
            AssertModelEqualsVM(item, vm);
        }

        [Fact]
        public void WithBasePropertiesSet()
        {
            Item item = new ItemBuilder().Random().Build();
            item.Attributes = null;
            item.Pictures = null;
            item.SubCategory = null;
            item.SubCategoryID = null;

            ItemVM vm = item.GetItemVM();

            Assert.Null(item.Attributes);
            Assert.Null(item.Pictures);
            Assert.Null(item.SubCategory);
            AssertModelEqualsVM(item, vm);
        }

        void AssertModelEqualsVM(Item item, ItemVM vm)
        {
            Assert.Equal(item.ID, vm.ID);
            Assert.Equal(item.Name, vm.Name);
            Assert.Equal(item.SKU, vm.SKU);
            Assert.Equal(item.Description, vm.Description);
            Assert.Equal(item.Price, vm.Price);
            Assert.Equal(item.Category.ID, vm.Category.ID);
            Assert.Equal(item.Category.Name, vm.Category.Name);
        }
    }
}
