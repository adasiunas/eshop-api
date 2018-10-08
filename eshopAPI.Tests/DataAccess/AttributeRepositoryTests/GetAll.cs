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
    public class GetAll
    {
        AttributeRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetAll()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "GetAll")
                .Options;
            _repository = GetAttributeRepository();

        }

        [Fact]
        public async void Success()
        {
            List<Attribute> attributes = await _repository.GetAll();
            Assert.Equal(3, attributes.Count);
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
