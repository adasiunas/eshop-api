using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
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

namespace eshopAPI.Tests.DataAccess.UserFeedbackRepositoryTests
{
    public class GetAllFeedbacksAsQueryable
    {
        UserFeedbackRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetAllFeedbacksAsQueryable()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "GetAllFeedbacksAsQueryable")
                .Options;
            _repository = GetUserFeedbackRepository();
        }

        [Fact]
        public async void Success()
        {
            List<UserFeedbackVM> feedbacks = (await _repository.GetAllFeedbacksAsQueryable()).ToList();
            Assert.Equal(5, feedbacks.Count);
        }

        UserFeedbackRepository GetUserFeedbackRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new UserFeedbackRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            UserFeedbackBuilder builder = new UserFeedbackBuilder();
            List<UserFeedbackEntry> entries = new List<UserFeedbackEntry>
            {
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            context.UserFeedbacks.AddRange(entries);
            context.SaveChanges();
        }
    }
}
