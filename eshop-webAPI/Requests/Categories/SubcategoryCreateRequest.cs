using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Requests.Categories
{
    public class SubcategoryCreateRequest
    {
        public int ParentID { get; set; }
        public string Name { get; set; }
    }
}
