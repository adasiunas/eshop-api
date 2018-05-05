using eshopAPI.Models;
using eshopAPI.Requests.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IShopUserRepository
    {
        Task<ShopUserProfile> GetUserProfile(string email);
        Task<bool> UpdateUserProfile(ShopUser user, UpdateUserRequest request);
        Task<ShopUser> GetUserWithEmail(string email);
        IQueryable<UserVM> GetAllUsersAsQueryable();
    }

    public class ShopUserRepository : IShopUserRepository
    {
        private readonly ShopContext _context;

        public ShopUserRepository(ShopContext context)
        {
            _context = context;
        }

        public async Task<ShopUserProfile> GetUserProfile(string email)
        {
            IQueryable<ShopUser> query = _context.Users.Where(u => u.NormalizedEmail.Equals(email.Normalize())).Include(user => user.Address);
            if (await query.CountAsync() != 1)
                return null;

            return query.First().GetUserProfile();
        }

        public async Task<bool> UpdateUserProfile(ShopUser user, UpdateUserRequest request)
        {
            if (user == null)
                return false;

            int updates;

            if (user.Address == null)
            {
                user.UpdateUserFromRequestCreateAddress(request);
                updates = await _context.SaveChangesAsync();
            }

            user.UpdateUserFromRequestUpdateAddress(request);
            updates = await _context.SaveChangesAsync();

            if (updates != 1 && updates != 2)
                return false;
            return true;
        }

        public async Task<ShopUser> GetUserWithEmail(string email)
        {
            var query = _context.Users.Where(u =>
                u.NormalizedEmail.Equals(email.Normalize())).Include(user => user.Address);

            if (await query.CountAsync() != 1)
                return null;
            return query.First();
        }



        public IQueryable<UserVM> GetAllUsersAsQueryable()
        {
            var query =
                from user in _context.Users
                join uRole in _context.UserRoles on user.Id equals uRole.UserId
                join role in _context.Roles on uRole.RoleId equals role.Id
                select new UserVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = $"{user.Name} {user.Surname}",
                    Role = role.Name
                };

            return query;
        }
    }
}

