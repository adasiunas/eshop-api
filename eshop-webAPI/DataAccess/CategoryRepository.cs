using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface ICategoryRepository
    {
        Task<Category> FindByIDAsync(long categoryID);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Category FindByName(string name);
        void Insert(Category category);
        void Update(Category category);
        void Save();
    }

    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(ShopContext context) : base(context)
        {
        }

        public async Task<Category> FindByIDAsync(long categoryID)
        {
            return await Context.Categories.FirstOrDefaultAsync(x => x.ID == categoryID);
        }

        public Category FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var a = Context.Categories
                .Where(x => x.ParentID == null)
                .Include(x => x.Children).ToList();

            foreach (var item in a)
            {
                item.Children.ToList().ForEach(x => x.Parent = null);
            }

            return a;
        }

        public void Insert(Category category)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
