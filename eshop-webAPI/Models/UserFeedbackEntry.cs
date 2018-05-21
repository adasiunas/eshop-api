using System.ComponentModel.DataAnnotations;

namespace eshopAPI.Models
{
    public class UserFeedbackEntry
    {
        [Key]
        public long ID { get; set; } // Primary key
        public string UserId { get; set; }
        public ShopUser User { get; set; }
        public string Message { get; set; }
        public int Rating { get; set; }
    }
}