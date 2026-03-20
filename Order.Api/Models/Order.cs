using System.ComponentModel.DataAnnotations;

namespace Orders.Api.Models;

public class Order {
    [Key]
    public Guid? Id { get; set; }
    public string ShippingAddress { get; set; }

    public Order(string shippingAddress) {
        ShippingAddress = shippingAddress;
    }
}