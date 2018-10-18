using eshopAPI.Models;
using eshopAPI.Requests.User;
using System;
using Xunit;

namespace eshopAPI.Tests.Models.ShopUserTests
{
    public class UpdateUserFromRequest
    {
        [Fact]
        public void UpdateUserFromRequestUpdateAddress_Success()
        {
            ShopUser user = new ShopUser
            {
                Address = new Address()
            };
            UpdateUserRequest request = CreateUpdateRequest();

            user.UpdateUserFromRequestUpdateAddress(request);

            AssertUserEqualRequest(user, request);
        }

        [Fact]
        public void UpdateUserFromRequestUpdateAddress_Fail()
        {
            ShopUser user = new ShopUser();
            UpdateUserRequest request = CreateUpdateRequest();

            Assert.Throws<NullReferenceException>(() => user.UpdateUserFromRequestUpdateAddress(request));
        }

        [Fact]
        public void UpdateUserFromRequestCreateAddress()
        {
            ShopUser user = new ShopUser();
            UpdateUserRequest request = CreateUpdateRequest();

            user.UpdateUserFromRequestCreateAddress(request);

            AssertUserEqualRequest(user, request);
        }

        UpdateUserRequest CreateUpdateRequest()
        {
            return new UpdateUserRequest
            {
                Name = "ABC",
                Surname = "DEF",
                Phone = "+123456789",
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
        
        void AssertUserEqualRequest(ShopUser user, UpdateUserRequest request)
        {
            Assert.Equal(user.Name, request.Name);
            Assert.Equal(user.Surname, request.Surname);
            Assert.Equal(user.Phone, request.Phone);
            Assert.Equal(user.Address.Name, request.Address.Name);
            Assert.Equal(user.Address.Surname, request.Address.Surname);
            Assert.Equal(user.Address.Street, request.Address.Street);
            Assert.Equal(user.Address.City, request.Address.City);
            Assert.Equal(user.Address.Country, request.Address.Country);
            Assert.Equal(user.Address.Postcode, request.Address.Postcode);
        }
    }
}
