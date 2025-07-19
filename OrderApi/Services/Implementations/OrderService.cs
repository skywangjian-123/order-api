using AutoMapper;
using OrderApi.Data.DTOs.Input;
using OrderApi.Data.DTOs.Output;
using OrderApi.Data.Models;
using OrderApi.Data.Repositories.Interfaces;
using OrderApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto orderDto)
        {
            try
            {
                // 验证输入
                ValidateOrderDto(orderDto);

                // 映射到领域模型
                var order = _mapper.Map<Order>(orderDto);

                // 创建订单
                var createdOrder = await _orderRepository.CreateOrderAsync(order);

                // 映射到响应DTO
                var responseDto = _mapper.Map<OrderResponseDto>(createdOrder);

                _logger.LogInformation("Order {OrderId} created successfully", responseDto.OrderId);

                return responseDto;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed for order creation: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order in service layer");
                throw;
            }
        }

        private void ValidateOrderDto(CreateOrderDto orderDto)
        {
            // 基本验证
            if (string.IsNullOrWhiteSpace(orderDto.CustomerName))
            {
                throw new ValidationException("Customer name is required");
            }

            if (orderDto.Items == null || !orderDto.Items.Any())
            {
                throw new ValidationException("Order must contain at least one item");
            }

            // 验证每个订单项
            foreach (var item in orderDto.Items)
            {
                if (item.ProductId == Guid.Empty)
                {
                    throw new ValidationException("ProductId is required for all items");
                }

                if (item.Quantity < 1)
                {
                    throw new ValidationException($"Quantity must be at least 1 for product {item.ProductId}");
                }
            }
        }
    }
}
