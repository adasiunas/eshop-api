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
        Task<List<SubCategory>> GetChildrenOfParent(int parentId);
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
            throw new NotImplementedException();
        }

        public Task<SubCategory> FindSubCategoryByID(long categoryId)
        {
            return Context.SubCategories.Include(sc => sc.Category).FirstOrDefaultAsync(x => x.ID == categoryId);
        }

        public Task<List<Category>> GetAllParentCategories()
        {
            return Context.Categories.Include(x => x.SubCategories).ToListAsync();
        }

        public async Task<List<SubCategory>> GetChildrenOfParent(int parentId)
        {
            var categoryList = (await Context.Categories
                .Where(x => x.ID == parentId)
                .Include(x => x.SubCategories)
                .Include("SubCategories.Items")
                .FirstOrDefaultAsync())?.SubCategories;

            return categoryList.ToList();
        }

        public Task<List<Category>> GetCategoriesWithSubcategories()
        {
            return Context.Categories.Include(c => c.SubCategories).ToListAsync();
        }

        public async Task<Category> InsertCategory(Category category)
        {
            return (await Context.Categories.AddAsync(category)).Entity;
        }

        public async Task<SubCategory> InsertSubcategory(SubCategory subCategory)
        {
            return (await Context.SubCategories.AddAsync(subCategory)).Entity;
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
