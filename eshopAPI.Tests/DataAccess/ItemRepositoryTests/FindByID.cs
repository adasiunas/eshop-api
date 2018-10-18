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
    public class FindByID
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindByID()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindByID")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            Item item = await _repository.FindByID(_firstItemId);
            Assert.Equal("ABC", item.SKU);
            Assert.Equal(3, item.Attributes.Count);
            Assert.Equal(2, item.Pictures.Count);
        }

        [Fact]
        public async void Failure()
        {
            Item item = await _repository.FindByID(_firstItemId - 1);
            Assert.Null(item);
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
                builder.New().SetSKU("ABC").AddAttributes(3).AddPictures(2).Build(),
                builder.Random().SetSKU("DEF").Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            _firstItemId = items.First().ID;

            context.Items.AddRange(items);
            context.SaveChanges();
        }
    }
}
