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
        Task<SubCategory> FindSubCategoryByIDAsync(long categoryId);
        Task<Category> FindByIDAsync(long categoryID);
        Task<List<Category>> GetAllParentCategoriesAsync();
        IEnumerable<SubCategory> GetChildrenOfParent(int parentId);
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

        public Task<Category> FindByIDAsync(long categoryID)
        {
            return Context.Categories.FirstOrDefaultAsync(x => x.ID == categoryID);
        }

        public Category FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<SubCategory> FindSubCategoryByIDAsync(long categoryId)
        {
            return await Context.SubCategories.FirstOrDefaultAsync(x => x.ID == categoryId);
        }

        public Task<List<Category>> GetAllParentCategoriesAsync()
        {
            return Context.Categories.ToListAsync();
        }

        public IEnumerable<SubCategory> GetChildrenOfParent(int parentId)
        {
            var categoryList = Context.Categories
                .Where(x => x.ID == parentId)
                .Include(x => x.SubCategories)
                .FirstOrDefault()?.SubCategories;

            return categoryList;
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
