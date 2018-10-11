using eshopAPI.Models;

namespace eshopAPI.Tests.Builders
{
    public class AttributeValueBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public string Value { get; set; } = "abc";

        static int _id = 1;

        AttributeValue _attributeValue;

        public AttributeValueBuilder()
        {
            _attributeValue = WithDefaultValues();
        }

        public AttributeValue Build()
        {
            _attributeValue.ID = ID;
            return _attributeValue;
        }

        AttributeValue WithDefaultValues()
        {
            Attribute attribute = new AttributeBuilder().Build();
            _attributeValue = new AttributeValue
            {
                Attribute = attribute,
                AttributeID = attribute.ID,
                Value = Value,
            };
            return _attributeValue;
        }
    }
}
