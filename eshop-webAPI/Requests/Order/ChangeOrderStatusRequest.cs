namespace eshopAPI.Requests.Order
{
    public class ChangeOrderStatusRequest
    {
        public long ID { get; set; }
        public string Status { get; set; }
    }
}
