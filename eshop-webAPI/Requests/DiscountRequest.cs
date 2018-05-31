using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Requests
{
    public class DiscountRequest
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public DateTime To { get; set; }
        public bool IsPercentages { get; set; }
        public long? ItemID { get; set; }
        public long? CategoryID { get; set; }
        public long? SubCategoryID { get; set; }
    }
}
