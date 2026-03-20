namespace Shipping.Api.Models;

public class Order {
    public Guid? Id { get; set; }
    public string ShippingAddress { get; set; }
    
    public Order() {
    }

    public Order(Guid? id, string shippingAddress) {
        Id = id;
        ShippingAddress = shippingAddress;
    }
}