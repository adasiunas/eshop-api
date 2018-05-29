using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderNumber { get; set; } // Bussiness key
        [Required]
        public ShopUser User { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateDate { get; set; }
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [Description("Processing")]
        Processing, // maybe something different
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
