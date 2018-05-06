using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface ICategoryRepository : IBaseRepository
    {
        Task<SubCategory> FindSubCategoryByID(long categoryId);
        Task<Category> FindByID(long categoryID);
        Task<List<Category>> GetAllParentCategories();
        Task<IEnumerable<SubCategory>> GetChildrenOfParent(int parentId);
        Task<Category> FindByName(string name);
        Task Insert(Category category);
        Task Update(Category category);
    }

    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(ShopContext context) : base(context)
        {
        }

        public Task<Category> FindByID(long categoryID)
        {
            return Context.Categories.FirstOrDefaultAsync(x => x.ID == categoryID);
        }

        public Task<Category> FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<SubCategory> FindSubCategoryByID(long categoryId)
        {
            return Context.SubCategories.Include(sc => sc.Category).FirstOrDefaultAsync(x => x.ID == categoryId);
        }

        public Task<List<Category>> GetAllParentCategories()
        {
            return Context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<SubCategory>> GetChildrenOfParent(int parentId)
        {
            var categoryList = (await Context.Categories
                .Where(x => x.ID == parentId)
                .Include(x => x.SubCategories)
                .FirstOrDefaultAsync())?.SubCategories;

            return categoryList;
        }

        public Task Insert(Category category)
        {
            throw new NotImplementedException();
        }

        public Task Update(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
