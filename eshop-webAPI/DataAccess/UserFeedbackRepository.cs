using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;

namespace eshopAPI.DataAccess
{
    public interface IUserFeedbackRepository
    {
        Task<UserFeedbackEntry> Insert(UserFeedbackEntry item);
        Task<IQueryable<UserFeedbackVM>> GetAllFeedbacksAsQueryable();
    }
    
    public class UserFeedbackRepository : BaseRepository, IUserFeedbackRepository
    {
        public UserFeedbackRepository(ShopContext context) : base(context)
        {
        }
        
        public async Task<UserFeedbackEntry> Insert(UserFeedbackEntry item)
        {
            return (await Context.UserFeedbacks.AddAsync(item)).Entity;
        }

        public Task<IQueryable<UserFeedbackVM>> GetAllFeedbacksAsQueryable()
        {
            var query = Context.UserFeedbacks.Select(uf => new UserFeedbackVM
            {
                ID = uf.ID,
                Email = uf.User.Email,
                Message = uf.Message,
                Rating = uf.Rating
            });

            return Task.FromResult(query);
        }
    }
}