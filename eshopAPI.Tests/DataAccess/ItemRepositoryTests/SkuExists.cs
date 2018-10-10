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
    public class SkuExists
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public SkuExists()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "SkuExists")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            bool exists = await _repository.SkuExists("ABC");
            Assert.True(exists);
        }

        [Fact]
        public async void Failure()
        {
            bool exists = await _repository.SkuExists("CBA");
            Assert.False(exists);
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
                builder.Random().SetSKU("GHI").Build(),
                builder.Random().SetSKU("JKL").Build(),
                builder.Random().SetSKU("MNO").Build()
            };
            _firstItemId = items.First().ID;

            context.Items.AddRange(items);
            context.SaveChanges();
        }
    }
}
