using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IAttributeRepository : IBaseRepository
    {
        Task<List<Attribute>> FindAttributeNamesByText(string text);
        Task<List<AttributeValue>> FindAttributeValuesById(int id);
        Task<Attribute> FindByName(string name);
        Task<Attribute> Insert(Attribute attribute);
        Task<List<Attribute>> GetAll();
    }

    public class AttributeRepository : BaseRepository, IAttributeRepository
    {
        public AttributeRepository(ShopContext context) : base(context)
        {
        }

        public Task<Attribute> FindByName(string name)
        {
            return Context.Attributes.Where(a => a.Name.ToLower().Equals(name.ToLower())).FirstOrDefaultAsync();
        }

        public Task<List<Attribute>> GetAll()
        {
            return Context.Attributes.ToListAsync();
        }

        public Task<List<Attribute>> FindAttributeNamesByText(string text)
        {
            return Context.Attributes
                .Where(x => x.Name.Contains(text))
                .ToListAsync();
        }

        public Task<Attribute> Insert(Attribute attribute)
        {
            return Task.FromResult(Context.Attributes.Add(attribute).Entity);
        }

        public Task<List<AttributeValue>> FindAttributeValuesById(int id)
        {
            return Context.AttributeValue
                .Where(x => x.AttributeID == id)
                .GroupBy(x => x.Value)
                .Select(x => x.First())
                .ToListAsync();
        }
    }
}
