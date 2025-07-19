using System.ComponentModel.DataAnnotations;

namespace OrderApi.DTOs.Input
{
    public class CreateOrderDto
    {
        [System.ComponentModel.DefaultValue("John Zhang")]
        [Required(ErrorMessage = "CustomerName is required")]
        public required string CustomerName { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
