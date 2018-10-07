using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Tests.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Models.CategoryTests
{
    public class GetCategoryVM
    {
        [Fact]
        public void Success()
        {

            Category category = new CategoryBuilder().AddSubCategories(3).Build();
            CategoryVM vm = category.GetCategoryVM();

            Assert.Equal(category.ID, vm.ID);
            Assert.Equal(category.Name, vm.Name);
            Assert.Equal(category.SubCategories.Count, vm.SubCategories.Count);
        }
    }
}
