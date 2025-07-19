using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Data.DTOs.Input
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }

        [DefaultValue(10)]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
