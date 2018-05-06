using eshopAPI.Models;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IAttributeRepository : IBaseRepository
    {
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
