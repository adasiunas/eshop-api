using eshopAPI.DataAccess;
using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.DataAccess.AttributeRepositoryTests
{
    public class FindAttributeValuesById
    {
        AttributeRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindAttributeValuesById()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindAttributeValuesById")
                .Options;
            _repository = GetAttributeRepository();

        }

        [Fact]
        public async void Success()
        {
            List<AttributeValue> attributes = await _repository.FindAttributeValuesById(1);

            Assert.Equal(4, attributes.Count);
        }

        private AttributeRepository GetAttributeRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new AttributeRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            context.Attributes.AddRange(new List<Attribute>
            {
                new Attribute { ID = 1, Name = "BBB" },
                new Attribute { ID = 2, Name = "ZZZ" },
                new Attribute { ID = 3, Name = "AAA" },
            });
            context.AttributeValue.AddRange(new List<AttributeValue>
            {
                new AttributeValue { ID = 1, AttributeID = 1, Value = "BBBaaa" },
                new AttributeValue { ID = 2, AttributeID = 1, Value = "BBBbbb" },
                new AttributeValue { ID = 3, AttributeID = 1, Value = "BBBccc" },
                new AttributeValue { ID = 4, AttributeID = 1, Value = "BBBddd" },
                new AttributeValue { ID = 5, AttributeID = 1, Value = "BBBddd" },
                new AttributeValue { ID = 6, AttributeID = 2, Value = "ZZZaaa" },
            });
            context.SaveChanges();
        }
    }
}
