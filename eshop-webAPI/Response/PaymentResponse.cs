namespace eshopAPI.Response
{
    public class PaymentResponse : BaseResponse
    {
        public string Id { get; set; }
        public string Created_At { get; set; }
        public int Amount { get; set; }
        public string Number { get; set; }
        public string Holder { get; set; }
        public int Exp_Year { get; set; }
        public int Exp_Month { get; set; }
        public string Cvv { get; set; }
    }

    public class BaseResponse
    {
        public int ResponseCode { get; set; }
        public bool IsSuccessfullResponse { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
    }
}