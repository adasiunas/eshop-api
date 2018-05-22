namespace eshopAPI.Requests
{
    public class UserFeedbackRequest
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public int Rating { get; set; }
    }
}