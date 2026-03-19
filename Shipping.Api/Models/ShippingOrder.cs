using System.ComponentModel.DataAnnotations;

namespace Shipping.Api.Models
{
    public class ShippingOrder
    {
        [Key]
        public Guid ShippingId { get; set; }
        public Guid? OrderId { get; set; }
        public String ShippingAdress { get; set; } = string.Empty;
        public ShippingStatus Status { get; set; } = ShippingStatus.Pending;

    }

    public enum ShippingStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }
}
