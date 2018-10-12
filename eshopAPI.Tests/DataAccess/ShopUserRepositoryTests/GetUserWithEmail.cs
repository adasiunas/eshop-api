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

namespace eshopAPI.Tests.DataAccess.ShopUserRepositoryTests
{
    public class GetUserWithEmail
    {
        string _firstUserId;
        string _userEmail;
        ShopUserRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetUserWithEmail()
        {
            _userEmail = "123@abc.xyz";
            _options = new DbContextOptionsBuilder<ShopContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "GetUserWithEmail")
                .Options;
            _repository = GetShopUserRepository();
        }

        [Fact]
        public async void Success()
        {
            ShopUser user = await _repository.GetUserWithEmail(_userEmail);
            Assert.Equal(_userEmail, user.Email);
            Assert.NotNull(user.Address);
        }

        [Fact]
        public async void Failure()
        {
            ShopUser user = await _repository.GetUserWithEmail(_userEmail+"zzzzzzz");
            Assert.Null(user);
        }

        ShopUser GetUserByEmail(string email)
        {
            ShopUser user;
            using (ShopContext context = new ShopContext(_options))
            {
                user = context.Users
                    .Where(o => o.Email == email)
                    .FirstOrDefault();
            }
            return user;
        }

        private ShopUserRepository GetShopUserRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new ShopUserRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            ShopUserBuilder builder = new ShopUserBuilder();
            List<ShopUser> users = new List<ShopUser>()
            {
                builder.Random().Build(),
                builder.Random().SetEmail(_userEmail).AddAddress().Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            _firstUserId = users.First().Id;

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
