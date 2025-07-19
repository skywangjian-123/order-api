using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.DTOs.Input
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }

        [System.ComponentModel.DefaultValue(10)]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
