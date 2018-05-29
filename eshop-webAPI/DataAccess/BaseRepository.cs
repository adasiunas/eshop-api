using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IBaseRepository
    {
        ShopContext Context { get; }
        Task<int> SaveChanges();
    }

    public class BaseRepository : IBaseRepository
    {
        public BaseRepository(ShopContext context)
        {
            Context = context;
        }

        public ShopContext Context { get; }

        public Task<int> SaveChanges()
        {
           return Context.SaveChangesAsync();
        }
    }
}
