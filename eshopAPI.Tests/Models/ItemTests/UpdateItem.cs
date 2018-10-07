using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Models
{
    public class UpdateItem
    {
        [Fact]
        public void Success()
        {
            Item itemA = new ItemBuilder().Build();
            Item itemB = new ItemBuilder().Random().Build();

            itemA.UpdateItem(itemB);
            
            Assert.Equal(itemB.Attributes, itemA.Attributes);
            Assert.Equal(itemB.Description, itemA.Description);
            Assert.Equal(itemB.Name, itemA.Name);
            Assert.Equal(itemB.Pictures, itemA.Pictures);
            Assert.Equal(itemB.Price, itemA.Price);
            Assert.Equal(itemB.SKU, itemA.SKU);
            Assert.Equal(itemB.CategoryID, itemA.CategoryID);
            Assert.Equal(itemB.SubCategoryID, itemA.SubCategoryID);
        }
    }
}
