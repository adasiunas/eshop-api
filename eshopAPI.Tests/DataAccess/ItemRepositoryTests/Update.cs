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
    public class Update
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public Update()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "Update")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            Item originalItem = new Item
            {
                ID = _firstItemId
            };
            Item newItem = new ItemBuilder().SetSKU("INS123").AddAttributes(4).AddPictures(1).AddCategory().AddSubCategory().Build();
            newItem.Timestamp = BitConverter.GetBytes(DateTime.Now.ToBinary()); 

            Item updatedItem = await _repository.Update(originalItem, newItem);
            await _repository.SaveChanges();

            Item foundItem = GetItemById(_firstItemId);

            Assert.Equal(newItem.SKU, updatedItem.SKU);
            Assert.Equal(4, updatedItem.Attributes.Count);
            Assert.Equal(1, updatedItem.Pictures.Count);
            Assert.Equal(newItem.CategoryID, updatedItem.CategoryID);
            Assert.Equal(newItem.SubCategoryID, updatedItem.SubCategoryID);

            Assert.Equal(newItem.SKU, foundItem.SKU);
            Assert.Equal(4, foundItem.Attributes.Count);
            Assert.Equal(1, foundItem.Pictures.Count);
            Assert.NotNull(foundItem.Category);
            Assert.NotNull(foundItem.SubCategory);
        }

        [Fact]
        public async void Failure()
        {
            Item originalItem = new Item
            {
                ID = _firstItemId - 1
            };
            Item newItem = new ItemBuilder().Random().Build();

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                Item updatedItem = await _repository.Update(originalItem, newItem);
                await _repository.SaveChanges();
            });
        }

        Item GetItemById(long id)
        {
            Item item;
            using (ShopContext context = new ShopContext(_options))
            {
                item = context.Items
                    .Where(o => o.ID == id)
                    .Include(o => o.Pictures)
                    .Include(o => o.Attributes)
                    .Include(o => o.Category)
                    .Include(o => o.SubCategory)
                    .FirstOrDefault();
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
