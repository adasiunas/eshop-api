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
    public class Insert
    {
        long _firstFeedbackId;
        UserFeedbackRepository _repository;
        DbContextOptions<ShopContext> _options;

        public Insert()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "Insert")
                .Options;
            _repository = GetUserFeedbackRepository();
        }

        [Fact]
        public async void Success()
        {
            UserFeedbackEntry newFeedback = new UserFeedbackBuilder().Random().Build();
            UserFeedbackEntry feedback = await _repository.Insert(newFeedback);
            await _repository.SaveChanges();
            UserFeedbackEntry foundFeedback = GetFeedbackById(newFeedback.ID);

            Assert.Equal(newFeedback.Message, feedback.Message);
            Assert.Equal(newFeedback.Rating, feedback.Rating);
            Assert.Equal(newFeedback.UserId, feedback.UserId);
            Assert.Equal(newFeedback.Message, foundFeedback.Message);
            Assert.Equal(newFeedback.Rating, foundFeedback.Rating);
            Assert.Equal(newFeedback.UserId, foundFeedback.UserId);
        }

        [Fact]
        public async void Failure()
        {
            UserFeedbackEntry newFeedback = new UserFeedbackEntry
            {
                ID = _firstFeedbackId
            };
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                UserFeedbackEntry feedback = await _repository.Insert(newFeedback);
                await _repository.SaveChanges();
            });
        }

        UserFeedbackEntry GetFeedbackById(long id)
        {
            UserFeedbackEntry feedback;
            using (ShopContext context = new ShopContext(_options))
            {
                feedback = context.UserFeedbacks.Where(o => o.ID == id).FirstOrDefault();
            }
            return feedback;
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
            _firstFeedbackId = entries.First().ID;

            context.UserFeedbacks.AddRange(entries);
            context.SaveChanges();
        }
    }
}
