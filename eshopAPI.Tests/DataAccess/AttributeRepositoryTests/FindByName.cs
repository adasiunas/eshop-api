using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.DataAccess.AttributeRepositoryTests
{
    public class FindByName
    {
        AttributeRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindByName()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindByName")
                .Options;
            _repository = GetAttributeRepository();

        }

        [Fact]
        public async void FindSuccess()
        {
            Attribute attribute = await _repository.FindByName("BBB");
            Assert.Equal("BBB", attribute.Name);
        }

        [Fact]
        public async void FindFailure()
        {
            Attribute attribute = await _repository.FindByName("DDD");
            Assert.Null(attribute);
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
