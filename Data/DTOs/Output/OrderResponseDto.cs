namespace OrderApi.Data.DTOs.Output
{
    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
