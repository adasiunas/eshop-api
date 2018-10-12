using eshopAPI.Models;
using eshopAPI.Tests.Helpers;
using System;

namespace eshopAPI.Tests.Builders
{
    public class AddressBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public string Name { get; set; } = "Name";
        public string Surname { get; set; } = "Surname";
        public string Street { get; set; } = "Street";
        public string City { get; set; } = "City";
        public string Country { get; set; } = "Country";
        public string Postcode { get; set; } = "Postcode";

        static int _id = 1;

        Address _address;

        public AddressBuilder()
        {
            _address = WithDefaultValues();
        }

        public Address Build()
        {
            _address.ID = ID;
            return _address;
        }

        public AddressBuilder Random()
        {
            Random rnd = new Random();
            _address = new Address
            {
                Name = rnd.RandomString(10),
                Surname = rnd.RandomString(10),
                Street = rnd.RandomString(10),
                City = rnd.RandomString(10),
                Country = rnd.RandomString(10),
                Postcode = rnd.RandomString(10)
            };
            return this;
        }

        Address WithDefaultValues()
        {
            _address = new Address
            {
                Name = Name,
                Surname = Surname,
                Street = Street,
                City = City,
                Country = Country,
                Postcode = Postcode
            };
            return _address;
        }
    }
}
