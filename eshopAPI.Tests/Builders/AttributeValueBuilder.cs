using eshopAPI.Models;

namespace eshopAPI.Tests.Builders
{
    class AttributeValueBuilder
    {
        AttributeValue _attributeValue;
        static int _id = 1;
        public long ID { get { return _id++; } }
        public long AttributeID { get; set; } = 1;
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
