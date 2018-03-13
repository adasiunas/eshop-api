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
        public long ID { get; set; } // Primary key
        [Required]
        public Guid OrderNumber { get; set; } // Bussiness key
        [Required]
        public long UserID { get; set; }
        [Required]
        public User User { get; set; }
        public DateTime CreateDate { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        [Required]
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
