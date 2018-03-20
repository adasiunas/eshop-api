namespace eshopAPI.Requests.Account
{
    public class ConfirmUserRequest
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }
}