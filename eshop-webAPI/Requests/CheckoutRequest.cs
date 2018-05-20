using eshopAPI.Models;

namespace eshopAPI.Requests
{
    public class CheckoutRequest
    {
        public long Amount { get; set; }
        public string Number { get; set; }
        public string Holder { get; set; }
        public int Exp_Year { get; set; }
        public int Exp_Month { get; set; }
        public string Cvv { get; set; }
        public Address Address { get; set; }
    }
}