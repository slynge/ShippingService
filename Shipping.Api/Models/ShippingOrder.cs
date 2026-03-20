using System.ComponentModel.DataAnnotations;

namespace Shipping.Api.Models;

public class ShippingOrder {
    [Key]
    public Guid ShippingId { get; set; }
    public Guid? OrderId { get; set; }
    public string ShippingAddress { get; set; }
    public ShippingStatus Status { get; set; }
    
    public ShippingOrder() {
    }

    public ShippingOrder(Guid? orderId, string shippingAddress, ShippingStatus status) {
        OrderId = orderId;
        ShippingAddress = shippingAddress;
        Status = status;
    }

    public ShippingOrder(Guid shippingId, Guid? orderId, string shippingAddress, ShippingStatus status) {
        ShippingId = shippingId;
        OrderId = orderId;
        ShippingAddress = shippingAddress;
        Status = status;
    }
}

public enum ShippingStatus {
    Pending,
    Shipped,
    Delivered,
    Cancelled
}