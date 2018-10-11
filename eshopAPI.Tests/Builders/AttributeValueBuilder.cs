using eshopAPI.Models;

namespace eshopAPI.Tests.Builders
{
    class AttributeValueBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public long AttributeID { get; set; } = 1;
        public Attribute Attribute { get; set; }
        public string Value { get; set; } = "abc";
        public string Name { get; set; } = "def";

        static int _id = 1;

        AttributeValue _attributeValue;

        public AttributeValueBuilder()
        {
            _attributeValue = WithDefaultValues();
        }
        private AttributeValue WithDefaultValues()
        {
            _attributeValue = new AttributeValue
            {
                AttributeID = AttributeID,
                Value = Value,
                Attribute = new Attribute
                {
                    ID = AttributeID,
                    Name = Name
                }
            };
            return _attributeValue;
        }

        public AttributeValue Build()
        {
            _attributeValue.ID = ID;
            return _attributeValue;
        }
    }
}
