using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class ItemAttributesVM
    {
        public long ID { get; set; }
        public long AttributeID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

    }
}
