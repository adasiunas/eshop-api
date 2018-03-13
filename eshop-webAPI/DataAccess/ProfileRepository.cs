using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IProfileRepository
    {
        Profile FindByID(long profileID);
        Profile FindByUserID(long userID);
        void Update(Profile profile);
        void Insert(Profile profile);
        void Save();
    }

    public class ProfileRepository : BaseRepository, IProfileRepository
    {
        public ProfileRepository(ShopContext context) : base(context)
        {
        }

        public Profile FindByID(long profileID)
        {
            throw new NotImplementedException();
        }

        public Profile FindByUserID(long userID)
        {
            throw new NotImplementedException();
        }

        public void Insert(Profile profile)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(Profile profile)
        {
            throw new NotImplementedException();
        }
    }
}
