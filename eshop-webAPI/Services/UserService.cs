using eshopAPI.Helpers;
using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Services
{
    public interface IUserService
    {
        User ValidateUser(string email, string password);
        string GetUserRole(User user);
    }

    public class UserService : IUserService
    {
        private readonly ShopContext _context;

        public UserService(ShopContext context)
        {
            _context = context;
        }

        public User ValidateUser(string email, string password)
        {
            User user = _context.Users.First(u => u.Email.Equals(email));
            if (null == user)
                return null;

            string providedHash = PasswordHash.ComputeHash(password);
            if (providedHash.Equals(user.Password))
                return user;

            return null;
        }

        public string GetUserRole(User user)
        {
            switch(user.Role)
            {
                case UserRole.User:
                    return "User";
                case UserRole.Admin:
                    return "Admin";
                case UserRole.Blocked:
                    return "Blocked";
                default:
                    return string.Empty;
            }
        }
    }
}
