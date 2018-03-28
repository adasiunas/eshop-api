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
        public virtual ICollection<Address> Addresses { get; set; }
    }

    public class ShopUserProfile
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }

    public static class ShopUserExtensions
    {
        public static ShopUserProfile GetUserProfile(this ShopUser user)
        {
            return new ShopUserProfile { Email = user.Email, Name = user.Name, Surname = user.Surname, Phone = user.Phone,  Addresses = user.Addresses};
        }

        public static ShopUser UpdateUserFromRequest(this ShopUser user, UpdateUserRequest request)
        {
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Phone = request.Phone;
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
