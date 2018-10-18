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

namespace eshopAPI.Tests.DataAccess.CartRepositoryTests
{
    public class RemoveCartItem
    {
        string _email = "test@test.com";
        long _firstCartItemId;
        CartRepository _repository;
        DbContextOptions<ShopContext> _options;

        public RemoveCartItem()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "RemoveCartItem")
                .Options;
            _repository = GetCartRepository();

        }

        [Fact]
        public async void RemoveSuccess()
        {
            long lastItemId = CartItemBuilder.LastId;
            CartItem item = new CartItem
            {
                ID = _firstCartItemId
            };

            await _repository.RemoveCartItem(item);
            await _repository.SaveChanges();

            Cart cart = await _repository.FindByUser(_email);
            Assert.Equal(2, cart.Items.Count);
        }

        [Fact]
        public async void RemoveFailure()
        {
            CartItem item = new CartItem
            {
                ID = _firstCartItemId - 1
            };

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await _repository.RemoveCartItem(item);
                await _repository.SaveChanges();
            });
        }

        private CartRepository GetCartRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new CartRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            ItemBuilder itemBuilder = new ItemBuilder();
            ShopUser user = new ShopUser
            {
                Id = "1",
                Name = "TestUser",
                Email = _email,
                NormalizedEmail = _email.Normalize()
            };
            context.Users.Add(user);
            CartItemBuilder cartItemBuilder = new CartItemBuilder();
            List<CartItem> items = new List<CartItem>
            {
                cartItemBuilder.Random().Build(),
                cartItemBuilder.Random().Build(),
                cartItemBuilder.Random().Build()
            };
            context.CartItems.AddRange(items);
            context.Carts.AddRange(new List<Cart>
            {
                new Cart { ID = 1, User = user, Items = items },
            });
            _firstCartItemId = items.First().ID;

            context.SaveChanges();
        }
    }
}
