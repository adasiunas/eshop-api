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
        Task<List<SubCategory>> GetChildrenOfParent(long parentId);
        Task<Category> FindByName(string name);
        Task<Category> InsertCategory(Category category);
        Task<SubCategory> InsertSubcategory(SubCategory subCategory);
        Task<List<Category>> GetCategoriesWithSubcategories();
        Task<Category> DeleteCategory(Category category);
        Task<SubCategory> DeleteSubcategory(SubCategory subCategory);
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
            return Context.Categories.FirstOrDefaultAsync(x => x.Name.Equals(name));
        }

        public async Task<List<SubCategory>> GetChildrenOfParent(long parentId)
        {
            var categoryList = (await Context.Categories
                .Where(x => x.ID == parentId)
                .Include(x => x.SubCategories)
                .ThenInclude(x => x.Items)
                .FirstOrDefaultAsync())?.SubCategories?.ToList();

            return categoryList;
        }

        public Task<SubCategory> FindSubCategoryByID(long categoryId)
        {
            return Context.SubCategories.Include(sc => sc.Category).FirstOrDefaultAsync(x => x.ID == categoryId);
        }

        public Task<List<Category>> GetCategoriesWithSubcategories()
        {
            return Context.Categories.Include(c => c.SubCategories).ToListAsync();
        }

        public Task<Category> InsertCategory(Category category)
        {
            return Task.FromResult(Context.Categories.Add(category).Entity);
        }

        public Task<SubCategory> InsertSubcategory(SubCategory subCategory)
        {
            return Task.FromResult(Context.SubCategories.Add(subCategory).Entity);
        }

        public Task<Category> DeleteCategory(Category category)
        {
            return Task.FromResult(Context.Categories.Remove(category).Entity);
        }

        public Task<SubCategory> DeleteSubcategory(SubCategory subCategory)
        {
            return Task.FromResult(Context.SubCategories.Remove(subCategory).Entity);
        }
    }
}
