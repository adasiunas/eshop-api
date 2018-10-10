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
    public class GetAllItemsSkuCodes
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetAllItemsSkuCodes()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "GetAllItemsSkuCodes")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            List<string> SKUs = (await _repository.GetAllItemsSkuCodes()).ToList();
            Assert.Equal("ABC", SKUs.ElementAt(0));
            Assert.Equal("DEF", SKUs.ElementAt(1));
            Assert.Equal("GHI", SKUs.ElementAt(2));
            Assert.Equal("JKL", SKUs.ElementAt(3));
            Assert.Equal("MNO", SKUs.ElementAt(4));
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
