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
        Task<Attribute> FindByID(long attributeID);
        Task<Attribute> FindByName(string name);
        Task Insert(Attribute attribute);
        Task Update(Attribute attribute);
    }

    public class AttributeRepository : BaseRepository, IAttributeRepository
    {
        public AttributeRepository(ShopContext context) : base(context)
        {
        }

        public Task<Attribute> FindByID(long attributeID)
        {
            throw new System.NotImplementedException();
        }

        public Task<Attribute> FindByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<Attribute>> FindAttributeNamesByText(string text)
        {
            return await Context.Attributes
                .Where(x => x.Name.Contains(text))
                .ToListAsync();
        }

        public Task Insert(Attribute attribute)
        {
            throw new System.NotImplementedException();
        }

        public Task Update(Attribute attribute)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<AttributeValue>> FindAttributeValuesById(int id)
        {
            return await Context.AttributeValue
                .Where(x => x.AttributeID == id)
                .ToListAsync();
        }
    }
}
