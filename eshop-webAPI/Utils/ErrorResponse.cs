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
        public const string EmptyRequest = nameof(EmptyRequest);
        public const string InvalidQuery = nameof(InvalidQuery);
        public const string EmptyFileError = nameof(EmptyFileError);
        public const string FileTooLargeError = nameof(FileTooLargeError);
        public const string BadFileContentType = nameof(BadFileContentType);
        public const string BadRequest = nameof(BadRequest);  // 400
        public const string ValidationFailed = nameof(ValidationFailed); //400
        public const string UserNotFound = nameof(UserNotFound);

        public const string Unauthorized = nameof(Unauthorized); //401
        public const string Forbidden = nameof(Forbidden); //403
        public const string NotFound = nameof(NotFound); // 404
        public const string GatewayTimeout = nameof(GatewayTimeout);  //408

        // Register validation 4**
        public const string PasswordNotMatchRePassword = nameof(PasswordNotMatchRePassword);
        public const string InvalidEmailOrPassword = nameof(InvalidEmailOrPassword);
        public const string InvalidPassword = nameof(InvalidPassword);
        public const string EmailNotConfirmed = nameof(EmailNotConfirmed);
        public const string AccountIsLocked = nameof(AccountIsLocked);
        public const string FailedToChangeUserRole = nameof(FailedToChangeUserRole);

        // Server Errors 5**
        public const string InternalServerError = nameof(InternalServerError); // 500
        public const string StorageErrorException = nameof(StorageErrorException); // 500
        public const string DbUpdateException = nameof(DbUpdateException);
    }
}