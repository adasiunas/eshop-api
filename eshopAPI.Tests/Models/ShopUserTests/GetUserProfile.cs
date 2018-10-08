using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Models.ShopUserTests
{
    public class GetUserProfile
    {
        [Fact]
        public void Success()
        {
            ShopUser user = CreateShopUser();
            ShopUserProfile profile = user.GetUserProfile();

            AssertUserEqualsProfile(user, profile);
        }

        ShopUser CreateShopUser()
        {
            return new ShopUser
            {
                Name = "ABC",
                Surname = "DEF",
                Phone = "+123456789",
                Email = "test@test.io",
                Address = new Address
                {
                    Name = "Street",
                    Surname = "Jacobs",
                    City = "Kansas",
                    Country = "USA",
                    Postcode = "US31323"
                }
            };
        }
        void AssertUserEqualsProfile(ShopUser user, ShopUserProfile profile)
        {
            Assert.Equal(user.Email, profile.Email);
            Assert.Equal(user.Name, profile.Name);
            Assert.Equal(user.Surname, profile.Surname);
            Assert.Equal(user.Phone, profile.Phone);
            Assert.Equal(user.Address, profile.Address);
        }
    }
}
