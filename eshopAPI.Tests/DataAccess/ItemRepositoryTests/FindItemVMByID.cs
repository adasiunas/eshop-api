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

namespace eshopAPI.Tests.DataAccess.ItemRepositoryTests
{
    public class FindItemVMByID
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindItemVMByID()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindItemVMByID")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            ItemVM item = await _repository.FindItemVMByID(_firstItemId);
            Assert.Equal("ABC", item.SKU);
            Assert.NotNull(item.Category);
            Assert.NotNull(item.SubCategory);
            Assert.Equal(3, item.Attributes.Count());
            Assert.Equal(2, item.Pictures.Count());
        }
        
        private ItemRepository GetItemRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new ItemRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            ItemBuilder builder = new ItemBuilder();
            List<Item> items = new List<Item>()
            {
                builder.New().SetSKU("ABC").AddCategory().AddSubCategory().AddAttributes(3).AddPictures(2).Build(),
                builder.Random().SetSKU("DEF").AddCategory().AddSubCategory().Build(),
                builder.Random().AddCategory().AddSubCategory().Build(),
                builder.Random().AddCategory().Build(),
                builder.Random().AddCategory().AddSubCategory().SetDeleted().Build()
            };
            _firstItemId = items.First().ID;

            context.Items.AddRange(items);
            context.SaveChanges();
        }
    }
}
