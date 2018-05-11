using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Requests.Categories
{
    public class CategoryUpdateRequest
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
