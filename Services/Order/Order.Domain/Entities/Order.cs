namespace Order.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCancelled { get; set; }
    }
}
