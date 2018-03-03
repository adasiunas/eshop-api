using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace eshopAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ShopContext _shopContext;

        public ValuesController(ShopContext shopContext)
        {
            _shopContext = shopContext;
            _shopContext.Database.EnsureCreated();

            if (_shopContext.Users.Count() == 0)
            {
                _shopContext.Users.Add(new User { ID = 0, Email = "test@test", Approved = true, Password = "Test", Role = 1});
                _shopContext.SaveChanges();
            }
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _shopContext.Users;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
