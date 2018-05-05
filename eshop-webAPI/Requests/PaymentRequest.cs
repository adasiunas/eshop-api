namespace eshopAPI.Requests
{
    public class PaymentRequest
    {
        public int Amount { get; set; }
        public string Number { get; set; }
        public string Holder { get; set; }
        public int Exp_Year { get; set; }
        public int Exp_Month { get; set; }
        public string Cvv { get; set; }
    }
}