namespace eshopAPI.Requests
{
    public class RegisterRequest
    {
        public string Username { get; set; } // Email
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}