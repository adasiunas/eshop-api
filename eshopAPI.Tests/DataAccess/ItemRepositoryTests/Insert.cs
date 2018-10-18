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
    public class Insert
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public Insert()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "Insert")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            Item item = new ItemBuilder().SetSKU("INS1").Build();
            Item newItem = await _repository.Insert(item);
            await _repository.SaveChanges();
            Item foundItem = GetItemById(item.ID);
            Assert.Equal(item.SKU, newItem.SKU);
            Assert.Equal(item.SKU, foundItem.SKU);
        }

        [Fact]
        public async void Failure()
        {
            Item item = new ItemBuilder().SetSKU("INS1").Build();
            item.ID = _firstItemId;

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                Item newItem = await _repository.Insert(item);
                await _repository.SaveChanges();
            });
        }

        Item GetItemById(long id)
        {
            Item item;
            using (ShopContext context = new ShopContext(_options))
            {
                item = context.Items.Where(o => o.ID == id).FirstOrDefault();
            }
            return item;
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
