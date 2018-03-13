using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public class BaseRepository
    {
        public BaseRepository(ShopContext context)
        {
            Context = context;
        }

        protected ShopContext Context { get; }
    }
}
