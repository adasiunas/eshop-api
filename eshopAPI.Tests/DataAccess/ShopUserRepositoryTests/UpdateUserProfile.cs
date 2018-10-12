using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.User;
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
    public class UpdateUserProfile
    {
        string _firstUserId;
        string _userEmail;
        ShopUserRepository _repository;
        DbContextOptions<ShopContext> _options;

        public UpdateUserProfile()
        {
            _userEmail = "123@abc.xyz";
            _options = new DbContextOptionsBuilder<ShopContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "UpdateUserProfile")
                .Options;
            _repository = GetShopUserRepository();
        }

        [Fact]
        public async void UserWithAddress()
        {
            ShopUser shopUser = GetUserByEmail(_userEmail);
            UpdateUserRequest request = new UpdateUserRequest
            {
                Name = "test",
                Surname = "tests",
                Phone = "123456789",
                Address = new AddressBuilder().Random().Build()
            };
            await _repository.UpdateUserProfile(shopUser, request);
            await _repository.SaveChanges();
            ShopUser foundUser = GetUserByEmail(_userEmail);

            AssertEqual(request, foundUser);
        }


        [Fact]
        public async void UserWithoutAddress()
        {
            ShopUser shopUser = GetUserById(_firstUserId);
            UpdateUserRequest request = new UpdateUserRequest
            {
                Name = "test",
                Surname = "tests",
                Phone = "123456789",
                Address = new AddressBuilder().Random().Build()
            };
            await _repository.UpdateUserProfile(shopUser, request);
            await _repository.SaveChanges();
            ShopUser foundUser = GetUserById(_firstUserId);

            AssertEqual(request, foundUser);
        }

        [Fact]
        public async void FailureWrongArgument()
        {
            UpdateUserRequest request = new UpdateUserRequest
            {
                Name = "test",
                Surname = "tests",
                Phone = "123456789",
                Address = new AddressBuilder().Random().Build()
            };
            await _repository.UpdateUserProfile(null, request);
            await _repository.SaveChanges();
            // if null was passed, error was handled and no action taken or exception thrown
            Assert.True(true);
        }

        [Fact]
        public async void FailureNotFound()
        {
            ShopUser shopUser = new ShopUser
            {
                Id = _firstUserId + "zzzzzz"
            };
            UpdateUserRequest request = new UpdateUserRequest
            {
                Name = "test",
                Surname = "tests",
                Phone = "123456789",
                Address = new AddressBuilder().Random().Build()
            };
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {

                await _repository.UpdateUserProfile(shopUser, request);
                await _repository.SaveChanges();
            });
        }

        void AssertEqual(UpdateUserRequest request, ShopUser user)
        {
            Assert.Equal(request.Name, user.Name);
            Assert.Equal(request.Surname, user.Surname);
            Assert.Equal(request.Phone, user.Phone);
            Assert.Equal(request.Address.Name, user.Address.Name);
            Assert.Equal(request.Address.Surname, user.Address.Surname);
            Assert.Equal(request.Address.Street, user.Address.Street);
            Assert.Equal(request.Address.Postcode, user.Address.Postcode);
            Assert.Equal(request.Address.ID, user.Address.ID);
        }

        ShopUser GetUserByEmail(string email)
        {
            ShopUser user;
            using (ShopContext context = new ShopContext(_options))
            {
                user = context.Users
                    .Include(o => o.Address)
                    .Where(o => o.Email == email)
                    .FirstOrDefault();
            }
            return user;
        }

        ShopUser GetUserById(string id)
        {
            ShopUser user;
            using (ShopContext context = new ShopContext(_options))
            {
                user = context.Users
                    .Include(o => o.Address)
                    .Where(o => o.Id == id)
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
