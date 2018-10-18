using eshopAPI.Models;

namespace eshopAPI.Tests.Builders
{
    public class AttributeBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public string Name { get; set; } = "def";

        static int _id = 1;

        Attribute _attribute;

        public AttributeBuilder()
        {
            _attribute = WithDefaultValues();
        }

        public Attribute Build()
        {
            _attribute.ID = ID;
            return _attribute;
        }

        Attribute WithDefaultValues()
        {
            _attribute = new Attribute
            {
                Name = Name
            };
            return _attribute;
        }
    }
}
