using eshopAPI.Requests.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class ShopUser : IdentityUser
    {
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string Surname { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        public Address Address { get; set; }
    }

    public class ShopUserProfile
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
    }

    public static class ShopUserExtensions
    {
        public static ShopUserProfile GetUserProfile(this ShopUser user)
        {
            return new ShopUserProfile { Email = user.Email, Name = user.Name, Surname = user.Surname, Phone = user.Phone,  Address = user.Address};
        }

        public static ShopUser UpdateUserFromRequestUpdateAddress(this ShopUser user, UpdateUserRequest request)
        {
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Phone = request.Phone;
            user.Address.Name = request.Address.Name;
            user.Address.Surname = request.Address.Surname;
            user.Address.Street = request.Address.Street;
            user.Address.City = request.Address.City;
            user.Address.Country = request.Address.Country;
            user.Address.Postcode = request.Address.Postcode;
            return user;
        }

        public static ShopUser UpdateUserFromRequestCreateAddress(this ShopUser user, UpdateUserRequest request)
        {
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Phone = request.Phone;
            Address newAddress = new Address()
            {
                Name = request.Address.Name,
                Surname = request.Address.Surname,
                Street = request.Address.Street,
                City = request.Address.City,
                Country = request.Address.Country,
                Postcode = request.Address.Postcode
            };
            user.Address = newAddress;
            return user;
        }
    }

    public enum UserRole
    {
        User,
        Admin,
        Blocked
    }
}
