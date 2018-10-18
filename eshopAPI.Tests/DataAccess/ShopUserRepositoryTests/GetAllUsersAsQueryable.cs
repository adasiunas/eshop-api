using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using eshopAPI.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.DataAccess.ShopUserRepositoryTests
{
    public class GetAllFeedbacksAsQueryable
    {
        string _firstUserId;
        string _userEmail;
        ShopUserRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetAllFeedbacksAsQueryable()
        {
            _userEmail = "123@abc.xyz";
            _options = new DbContextOptionsBuilder<ShopContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "GetAllUsersAsQueryable")
                .Options;
            _repository = GetShopUserRepository();
        }

        [Fact]
        public async void Success()
        {
            List<UserVM> user = (await _repository.GetAllUsersAsQueryable()).ToList();
            Assert.Equal(5, user.Count);
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
                builder.Random().SetEmail(_userEmail).Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            context.Users.AddRange(users);
            _firstUserId = users.First().Id;

            IdentityRole role = new IdentityRole
            {
                Id = "1",
                Name = "User"
            };
            context.Roles.Add(role);

            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();
            users.ForEach(u => userRoles.Add(new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = u.Id
            }));
            context.UserRoles.AddRange(userRoles);

            context.SaveChanges();
        }
    }
}
