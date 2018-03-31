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
        Task<bool> DeleteAddress(string email, long addressID);
        Task<bool> AddAddress(string email, AddressRequest newAddress);
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
                .Include(u => u.Addresses);

            if (await query.CountAsync() != 1)
                return false;

            ShopUser user = query.First();
            ICollection<Address> addresses = user.Addresses;

            foreach (Address address in addresses)
            {
                if (address.ID != request.ID)
                    continue;
                address.Name = request.Name;
                address.Surname = request.Surname;
                address.Street = request.Street;
                address.City = request.City;
                address.Postcode = request.Postcode;
                address.Country = request.Country;
                break;
            }

            user.Addresses = addresses;

            int updates = await _context.SaveChangesAsync();
            if (updates != 1)
                return false;
            return true;
        }

        public async Task<bool> DeleteAddress(string email, long id)
        {
            var user = await GetUserWithEmail(email);
            if (user == null)
                return false;

            user.Addresses.Remove(user.Addresses.Where(a => a.ID == id).First());

            int updates = await _context.SaveChangesAsync();
            if (updates != 1)
                return false;
            return true;
        }

        public async Task<bool> AddAddress(string email, AddressRequest request)
        {
            var user = await GetUserWithEmail(email);
            if (user == null)
                return false;

            Address address = AddressExtensions.GetAddressFromRequest(request);

            user.Addresses.Add(address);

            int updates = await _context.SaveChangesAsync();
            if (updates != 1)
                return false;
            return true;
        }

        private async Task<ShopUser> GetUserWithEmail(string email)
        {
            var query = _context.Users.Where(u =>
                u.NormalizedEmail.Equals(email.Normalize()))
                .Include(u => u.Addresses);

            if (await query.CountAsync() != 1)
                return null;
            return query.First();
        }
    }
}
