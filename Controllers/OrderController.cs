using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data.DTOs.Input;
using OrderApi.Data.DTOs.Output;
using OrderApi.Data.Models;
using OrderApi.Data.Repositories.Interfaces;
using OrderApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
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
                var responseDto = await _orderService.CreateOrderAsync(orderDto);

                var locationUri = Url.Action(nameof(GetOrder), new { id = responseDto.OrderId })!;

                return Created(locationUri, responseDto);
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
