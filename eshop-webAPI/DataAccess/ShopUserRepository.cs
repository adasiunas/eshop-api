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
        Task<bool> UpdateUserProfile(string email, UpdateUserRequest request);
        Task<bool> UpdateUserAddress(string email, AddressRequest request);
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
            IQueryable<ShopUser> query = _context.Users.Where(u => u.NormalizedEmail.Equals(email.Normalize())).Include(user => user.Addresses);
            if (await query.CountAsync() != 1)
                return null;

            return query.First().GetUserProfile();
        }

        public async Task<bool> UpdateUserProfile(string email, UpdateUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail.Equals(email.Normalize()));
            if (user == null)
                return false;
            user.UpdateUserFromRequest(request);
            int updates = await _context.SaveChangesAsync();
            if (updates != 1)
                return false;
            return true;
        }

        public async Task<bool> UpdateUserAddress(string email, AddressRequest request)
        { 
            var query = _context.Users.Where(u => 
                u.NormalizedEmail.Equals(email.Normalize()))
                .Include(user => user.Addresses)
                .Select(u => u.Addresses
                .Where(a => a.ID == request.Id));

            if (await query.CountAsync() != 1)
                return false;

            query.First().First().Name = request.Name;
            query.First().First().Surname = request.Surname;
            query.First().First().Street = request.Street;
            query.First().First().City = request.City;
            query.First().First().Postcode = request.Postcode;
            query.First().First().Country = request.Country;

            int updates = await _context.SaveChangesAsync();
            if (updates != 1)
                return false;
            return true;
            
        }
    }
}
