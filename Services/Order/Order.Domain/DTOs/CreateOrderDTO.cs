namespace Order.Domain.DTOs
{
    public class CreateOrderDTO
    {       
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
