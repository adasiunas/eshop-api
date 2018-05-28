using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels.Admin
{
    public class AdminOrderVM
    {
        public long ID { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserEmail { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
