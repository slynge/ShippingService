namespace Orders.Api.Models
{
    public class Order
    {
        public Guid? Id { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
