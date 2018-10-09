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
    public class FindByID
    {
        long _firstCategoryId;
        CategoryRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindByID()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindByID")
                .Options;
            _repository = GetCategoryRepository();
        }

        [Fact]
        public async void Success()
        {
            Category category = await _repository.FindByID(_firstCategoryId);
            Assert.NotNull(category);
            Assert.Equal("ABC", category.Name);
            Assert.Equal(_firstCategoryId, category.ID);
        }

        [Fact]
        public async void Failure()
        {
            Category category = await _repository.FindByID(_firstCategoryId - 1);
            Assert.Null(category);
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
