using eshopAPI.Models;

namespace eshopAPI.Tests.Builders
{
    class AttributeValueBuilder
    {
        AttributeValue _attributeValue;
        public long ID { get; set; } = 1;
        public long AttributeID { get; set; } = 2;
        public Attribute Attribute { get; set; }
        public string Value { get; set; } = "abc";
        public string Name { get; set; } = "def";

        public AttributeValueBuilder()
        {
            _attributeValue = WithDefaultValues();
        }
        public AttributeValue WithDefaultValues()
        {
            _attributeValue = new AttributeValue
            {
                ID = ID,
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
    }
}
