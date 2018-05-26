using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels
{
    public class OrderVM
    {
        public long ID { get; set; }
        public Guid OrderNumber { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual IEnumerable<OrderItemVM> Items { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
