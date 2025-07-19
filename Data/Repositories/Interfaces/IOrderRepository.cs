using OrderApi.Data.Models;

namespace OrderApi.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
    }
}
