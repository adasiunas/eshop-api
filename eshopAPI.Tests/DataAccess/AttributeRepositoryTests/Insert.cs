using eshopAPI.DataAccess;
using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.DataAccess.AttributeRepositoryTests
{
    public class Insert
    {
        AttributeRepository _repository;
        DbContextOptions<ShopContext> _options;

        public Insert()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "Insert")
                .Options;
            _repository = GetAttributeRepository();

        }

        [Fact]
        public async void Success()
        {
            Attribute newAttribute = new Attribute()
            {
                Name = "DDD",
                ID = 4
            };
            Attribute attribute = await _repository.Insert(newAttribute);
            await _repository.SaveChanges();
            Assert.Equal(4, attribute.ID);
            Assert.Equal("DDD", attribute.Name);
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
            context.SaveChanges();
        }
    }
}
