using eshopAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IAttributeRepository : IBaseRepository
    {
        Task<Attribute> FindByID(long attributeID);
        Task<Attribute> FindByName(string name);
        Task Insert(Attribute attribute);
        Task Update(Attribute attribute);
        Task<List<Attribute>> GetAll();
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
            return  Task.FromResult(Context.Attributes.Where(a => a.Name.Equals(name)).First());
        }

        public Task<List<Attribute>> GetAll()
        {
            return Task.FromResult(Context.Attributes.ToList());
        }

        public Task Insert(Attribute attribute)
        {
            throw new System.NotImplementedException();
        }

        public Task Update(Attribute attribute)
        {
            throw new System.NotImplementedException();
        }
    }
}
