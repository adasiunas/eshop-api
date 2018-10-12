using eshopAPI.Models;
using eshopAPI.Tests.Helpers;
using System;

namespace eshopAPI.Tests.Builders
{
    public class UserFeedbackBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public string Message { get; set; } = "Very good review";
        public int Rating { get; set; } = 9;

        static int _id = 1;

        UserFeedbackEntry _feedback;

        public UserFeedbackBuilder()
        {
            _feedback = WithDefaultValues();
        }

        public UserFeedbackBuilder New()
        {
            _feedback = WithDefaultValues();
            return this;
        }

        public UserFeedbackEntry Build()
        {
            _feedback.ID = ID;
            return _feedback;
        }

        public UserFeedbackBuilder Random()
        {
            Random rnd = new Random();
            _feedback = new UserFeedbackEntry
            {
                Message = rnd.RandomString(15),
                Rating = rnd.Next(1, 10)
            };
            ShopUser user = new ShopUserBuilder().Random().Build();
            SetUser(user);
            return this;
        }

        public UserFeedbackBuilder SetUser(ShopUser user)
        {
            _feedback.User = user;
            _feedback.UserId = user.Id;
            return this;
        }



        UserFeedbackEntry WithDefaultValues()
        {
            _feedback = new UserFeedbackEntry
            {
                Message = Message,
                Rating = Rating
            };
            return _feedback;
        }
    }
}
