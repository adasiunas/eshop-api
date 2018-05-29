namespace eshopAPI.Models.ViewModels
{
    public class UserFeedbackVM
    {
        public long ID { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public int Rating { get; set; }
    }
}