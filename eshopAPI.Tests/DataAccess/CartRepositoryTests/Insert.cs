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
    public class Insert
    {
        string _email = "test@test.com";
        CartRepository _repository;
        DbContextOptions<ShopContext> _options;

        public Insert()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "Insert")
                .Options;
            _repository = GetCartRepository();
        }

        [Fact]
        public async void Success()
        {
            Cart cart = CreateCart(2);

            await _repository.Insert(cart);
            await _repository.SaveChanges();

            cart = GetCart(2);
            Assert.NotNull(cart);
        }

        [Fact]
        public async void Failure()
        {
            Cart cart = CreateCart(1);

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await _repository.Insert(cart);
                await _repository.SaveChanges();
            });
        }

        Cart GetCart(long id)
        {
            Cart cart;
            using (ShopContext dbContext = new ShopContext(_options))
            {
                cart = dbContext.Carts
                    .Include(o => o.Items)
                    .Where(o => o.ID == id)
                    .FirstOrDefault();
            }
            return cart;
        }
        
        Cart CreateCart(int id)
        {
            ShopUser user = new ShopUser
            {
                Id = id.ToString(),
                Name = "TestUser",
                Email = _email,
                NormalizedEmail = _email.Normalize()
            };

            return new Cart
            {
                ID = id,
                User = user,
                Items = new List<CartItem>()
            };
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
            context.SaveChanges();
        }
    }
}
