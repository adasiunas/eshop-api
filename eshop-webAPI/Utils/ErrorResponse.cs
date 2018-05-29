namespace eshopAPI.Utils
{
    public class ErrorResponse
    {
        public string Reason { get; set; }
        public string Message { get; set; }

        public ErrorResponse(string _reason, string _message)
        {
            Reason = _reason;
            Message = _message;
        }

        public ErrorResponse(){}
    }

    public class ErrorReasons
    {
        public const string BadRequest = nameof(BadRequest);  // 400

        public const string Unauthorized = nameof(Unauthorized); //401
        public const string Forbidden = nameof(Forbidden); //403
        public const string NotFound = nameof(NotFound); // 404
        public const string InvalidEmailOrPassword = nameof(InvalidEmailOrPassword);
        public const string EmailNotConfirmed = nameof(EmailNotConfirmed);
        public const string AccountIsNotConfirmed = nameof(AccountIsNotConfirmed);
        public const string DbUpdateException = nameof(DbUpdateException);
        public const string UserIsBlocked = nameof(UserIsBlocked);
    }
}