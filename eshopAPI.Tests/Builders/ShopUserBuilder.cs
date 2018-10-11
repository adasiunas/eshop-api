using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class ShopUserBuilder
    {
        public string Name { get; set; } = "CatName";
        public string Email { get; set; } = "test@test.com";

        ShopUser _user;

        public ShopUserBuilder()
        {
            _user = WithDefaultValues();
        }

        public ShopUserBuilder New()
        {
            _user = WithDefaultValues();
            return this;
        }

        ShopUser WithDefaultValues()
        {
            _user = new ShopUser
            {
                Name = Name,
                Email = Email,
                NormalizedEmail = Email.Normalize()
            };
            return _user;
        }

        public ShopUserBuilder SetName(string name)
        {
            _user.Name = name;
            return this;
        }

        public ShopUser Build()
        {
            _user.Id = Guid.NewGuid().ToString();
            return _user;
        }
    }
}
