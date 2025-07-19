using OrderApi.Data.Models;
using OrderApi.Data.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Data.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(AppDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            ValidateOrderItems(order.Items);
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Created order {OrderId}", order.OrderId);
                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        private void ValidateOrderItems(List<OrderItem> items)
        {
            if (items == null || items.Count == 0)
            {
                throw new ValidationException("Order must contain at least one item");
            }

            foreach (var item in items)
            {
                var validationContext = new ValidationContext(item);
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(item, validationContext, validationResults, true))
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    throw new ValidationException($"Invalid order item: {string.Join(", ", errorMessages)}");
                }
            }
        }
    }
}
