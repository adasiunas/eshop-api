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
    public class FindAttributeNamesByText
    {
        AttributeRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindAttributeNamesByText()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindAttributeNamesByText")
                .Options;
            _repository = GetAttributeRepository();

        }

        [Fact]
        public async void FindSuccess()
        {
            List<Attribute> attributes = await _repository.FindAttributeNamesByText("B");
            Assert.Single(attributes);
            Assert.Equal("BBB", attributes.First().Name);
        }

        [Fact]
        public async void FindFailure()
        {
            List<Attribute> attributes = await _repository.FindAttributeNamesByText("D");
            Assert.Empty(attributes);
        }
        
        private AttributeRepository GetAttributeRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            SeedData(dbContext);
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
