using eshopAPI.Models;

namespace eshopAPI.DataAccess
{
    public interface IAttributeRepository : IBaseRepository
    {
        Attribute FindByID(long attributeID);
        Attribute FindByName(string name);
        void Insert(Attribute attribute);
        void Update(Attribute attribute);
        void Save();
    }

    public class AttributeRepository : BaseRepository, IAttributeRepository
    {
        public AttributeRepository(ShopContext context) : base(context)
        {
        }

        public Attribute FindByID(long attributeID)
        {
            throw new System.NotImplementedException();
        }

        public Attribute FindByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(Attribute attribute)
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }

        public void Update(Attribute attribute)
        {
            throw new System.NotImplementedException();
        }
    }
}
