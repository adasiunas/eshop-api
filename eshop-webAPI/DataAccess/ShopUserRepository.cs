using eshopAPI.Models;
using eshopAPI.Requests.User;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IShopUserRepository : IBaseRepository
    {
        Task<ShopUserProfile> GetUserProfile(string email);
        Task UpdateUserProfile(ShopUser user, UpdateUserRequest request);
        Task<ShopUser> GetUserWithEmail(string email);
        Task<IQueryable<UserVM>> GetAllUsersAsQueryable();
    }

    public class ShopUserRepository : BaseRepository, IShopUserRepository
    {
        public ShopUserRepository(ShopContext context) : base(context)
        {
        }

        public async Task<ShopUserProfile> GetUserProfile(string email)
        {
            IQueryable<ShopUser> query = Context.Users.Where(u => u.NormalizedEmail.Equals(email.Normalize())).Include(user => user.Address);
            if (await query.CountAsync() != 1)
                return null;

            return query.First().GetUserProfile();
        }

        public Task UpdateUserProfile(ShopUser user, UpdateUserRequest request)
        {
            if (user == null)
                return Task.CompletedTask;

            if (user.Address == null)
            {
                user.UpdateUserFromRequestCreateAddress(request);
            }
            else
            {
                user.UpdateUserFromRequestUpdateAddress(request);
            }
            return Task.CompletedTask;
        }

        public Task<ShopUser> GetUserWithEmail(string email)
        {
            return Context.Users.Where(u =>
                u.NormalizedEmail.Equals(email.Normalize())).Include(user => user.Address).FirstOrDefaultAsync();
        }
        
        public Task<IQueryable<UserVM>> GetAllUsersAsQueryable()
        {
            var query =
                from user in Context.Users
                join uRole in Context.UserRoles on user.Id equals uRole.UserId
                join role in Context.Roles on uRole.RoleId equals role.Id
                select new UserVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = role.Name,
                    MoneySpent = user.Orders.SelectMany(x => x.Items).Select(x => x.Price * x.Count).Sum(),
                    OrderCount = user.Orders.Count,
                    AverageMoneySpent = user.Orders.Count > 0 ? user.Orders.SelectMany(x => x.Items).Select(x => x.Price * x.Count).Sum() / user.Orders.Count : 0
                };

            return Task.FromResult(query);
        }
    }
}

