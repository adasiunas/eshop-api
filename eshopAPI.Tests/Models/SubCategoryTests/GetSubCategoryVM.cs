using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Models.SubCategoryTests
{
    public class GetSubCategoryVM
    {
        [Fact]
        public void Success()
        {
            SubCategory subCategory = new SubCategoryBuilder().Build();
            SubCategoryVM vm = subCategory.GetSubCategoryVM();

            Assert.Equal(subCategory.ID, vm.ID);
            Assert.Equal(subCategory.Name, vm.Name);
        }
    }
}
