using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IUserRepository
    {
        IQueryable<UserVM> GetAllUsersAsQueryable(); 
    }

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ShopContext context) : base(context)
        {

        }

        public IQueryable<UserVM> GetAllUsersAsQueryable()
        {
            var query =
                from user in Context.Users
                join uRole in Context.UserRoles on user.Id equals uRole.UserId
                join role in Context.Roles on uRole.RoleId equals role.Id
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
