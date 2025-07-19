using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderApi.DTOs.Input;
using OrderApi.DTOs.Output;
using OrderApi.Models;
using OrderApi.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderRepository repository,
            IMapper mapper,
            ILogger<OrdersController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            _logger.LogInformation("Creating new order for {Customer}", orderDto.CustomerName);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid order data received");
                return BadRequest(ModelState);
            }

            try
            {
                var order = _mapper.Map<Order>(orderDto);
                var createdOrder = await _repository.CreateOrderAsync(order);
                var response = _mapper.Map<OrderResponseDto>(createdOrder);

                return CreatedAtAction(
                    nameof(GetOrder),
                    new { id = createdOrder.OrderId },
                    response);
            }
            catch (ValidationException ex) // 捕获验证异常
            {
                _logger.LogWarning("Validation failed: {Message}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Order creation failed");
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating order");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            // Implementation for GET
            return Ok();
        }
    }
}
