using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IUserRepository
    {
        User FindByEmail(string email);
        User FindByID(long userID);
        void Insert(User user);
        void Update(User user);
        void Save();
    }

    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ShopContext context) : base(context)
        {
        }

        public User FindByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public User FindByID(long userID)
        {
            throw new NotImplementedException();
        }

        public void Insert(User user)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
