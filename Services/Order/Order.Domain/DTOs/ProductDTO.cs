namespace Order.Domain.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
    }
}
