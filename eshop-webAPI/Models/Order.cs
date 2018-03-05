using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class Order
    {
        [Key]
        public long ID { get; set; }
        public long UserID { get; set; }
        public User User { get; set; }
        public DateTime CreateDate { get; set; }
        public OrderStatus Status { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }
    }

    public enum OrderStatus
    {
        Accepted,
        InProgress, // maybe something different
        Sent,
        Delivered
    }
}
