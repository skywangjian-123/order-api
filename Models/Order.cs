namespace OrderApi.Models
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public required string CustomerName { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();
    }
}
