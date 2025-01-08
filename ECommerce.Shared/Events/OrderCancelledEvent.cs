namespace ECommerce.Shared.Events
{
    public class OrderCancelledEvent
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int ReturnQuantity { get; set; }
        public DateTime CancelledAt { get; set; }
    }
}
