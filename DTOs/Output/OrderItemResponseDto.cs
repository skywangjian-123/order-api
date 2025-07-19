namespace OrderApi.DTOs.Output
{
    public class OrderItemResponseDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
