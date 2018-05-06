using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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
        public ShopUser User { get; set; }
        public DateTime CreateDate { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        [Required]
        public virtual ICollection<OrderItem> Items { get; set; }
        [Required]
        public string DeliveryAddress { get; set; }
    }

    public enum OrderStatus
    {
        [Description("Accepted")]
        Accepted,
        [Description("In progress")]
        InProgress, // maybe something different
        [Description("Sent")]
        Sent,
        [Description("Delivered")]
        Delivered
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this OrderStatus value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attribs.Length > 0)
            {
                return ((DescriptionAttribute)attribs[0]).Description;
            }
            return value.ToString();
        }
    }
}
