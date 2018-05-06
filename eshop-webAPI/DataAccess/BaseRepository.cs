using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IBaseRepository
    {
        Task<int> SaveChanges();
    }

    public class BaseRepository : IBaseRepository
    {
        public BaseRepository(ShopContext context)
        {
            Context = context;
        }

        protected ShopContext Context { get; }

        public Task<int> SaveChanges()
        {
           return Context.SaveChangesAsync();
        }
    }
}
