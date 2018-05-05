using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace eshopAPI.DataAccess
{
    public interface ICategoryRepository : IBaseRepository
    {
        Category FindByID(long categoryID);
        Category FindByName(string name);
        void Insert(Category category);
        void Update(Category category);
        IEnumerable<Category> GetAllCategories();
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

        public IEnumerable<Category> GetAllCategories()
        {
            var query = from c in Context.Categories
                select new Category
                {
                    Name = c.Name,
                    ID = c.ID,
                    SubCategories = c.SubCategories.OrderBy(sc => sc.Name).ToList()
                };

            return query;
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
