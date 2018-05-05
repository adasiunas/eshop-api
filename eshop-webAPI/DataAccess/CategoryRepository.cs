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
        Category FindByID(long categoryID);
        Category FindByName(string name);
        Task<SubCategory> FindSubCategoryByID(long ID);
        void Insert(Category category);
        void Update(Category category);
    }

    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(ShopContext context) : base(context)
        {
        }

        public Category FindByID(long categoryID)
        {
            throw new NotImplementedException();
        }

        public Category FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<SubCategory> FindSubCategoryByID(long ID)
        {
            return await Context.SubCategories.Include(sc => sc.Category).Where(sc => sc.ID == ID).FirstOrDefaultAsync();
        }

        public void Insert(Category category)
        {
            throw new NotImplementedException();
        }

        public void Update(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
