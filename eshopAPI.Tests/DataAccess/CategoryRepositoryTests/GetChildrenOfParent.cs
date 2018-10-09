using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using eshopAPI.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.DataAccess.CategoryRepositoryTests
{
    public class GetChildrenOfParent
    {
        long _firstCategoryId;
        CategoryRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetChildrenOfParent()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "GetChildrenOfParent")
                .Options;
            _repository = GetCategoryRepository();
        }

        [Fact]
        public async void Success()
        {
            List<SubCategory> subCategories = await _repository.GetChildrenOfParent(_firstCategoryId);
            Assert.Equal(2, subCategories.Count);
        }

        [Fact]
        public async void Failure()
        {
            List<SubCategory> subCategories = await _repository.GetChildrenOfParent(_firstCategoryId - 1);
            Assert.Null(subCategories);
        }

        private CategoryRepository GetCategoryRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new CategoryRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            CategoryBuilder categoryBuilder = new CategoryBuilder();
            List<Category> categories = new List<Category>()
            {
                categoryBuilder.New().SetName("ABC").AddSubCategories(2).Build(),
                categoryBuilder.New().SetName("DEF").AddSubCategories(4).Build(),
                categoryBuilder.New().SetName("GHI").Build(),
            };
            _firstCategoryId = categories.First().ID;
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
