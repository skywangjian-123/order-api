using OrderApi.Data.DTOs.Input;
using OrderApi.Data.DTOs.Output;

namespace OrderApi.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto orderDto);
    }
}
