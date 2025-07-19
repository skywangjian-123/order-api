using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrderApi.Controllers;
using OrderApi.Data.DTOs.Output;
using OrderApi.Services.Interfaces;
using OrderApi.Tests.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Tests.Controllers
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private Mock<IOrderService> _mockService;
        private OrdersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IOrderService>();
            var logger = TestHelpers.CreateTestLogger<OrdersController>();
            _controller = new OrdersController(_mockService.Object, logger);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost:5001");

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Orders");

            var actionDescriptor = new ControllerActionDescriptor
            {
                AttributeRouteInfo = new AttributeRouteInfo
                {
                    Template = "api/orders"
                }
            };

            var actionContext = new ActionContext(
                httpContext,
                routeData,
                actionDescriptor);

            _controller.ControllerContext = new ControllerContext(actionContext);
            _controller.Url = TestHelpers.CreateMockUrlHelper();
        }


        [Test]
        public async Task CreateOrder_ValidInput_ReturnsCreatedResult()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            var responseDto = new OrderResponseDto
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CreatedTime = DateTime.UtcNow,
                Items = new List<OrderItemResponseDto>
            {
                new OrderItemResponseDto
                {
                    ProductId = orderDto.Items[0].ProductId,
                    Quantity = 5
                }
            }
            };

            _mockService.Setup(s => s.CreateOrderAsync(orderDto))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<CreatedResult>();
            var createdAtResult = result as CreatedResult;
            createdAtResult.Value.ShouldBeEquivalentWithDetails(responseDto);
        }

        [Test]
        public async Task CreateOrder_ValidationError_ReturnsBadRequest()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            orderDto.CustomerName = "";

            _mockService.Setup(s => s.CreateOrderAsync(orderDto))
                .ThrowsAsync(new ValidationException("Validation failed"));

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest.Value.Should().BeOfType<ProblemDetails>();
            var problemDetails = badRequest.Value as ProblemDetails;
            problemDetails.Detail.Should().Be("Validation failed");
        }

        [Test]
        public async Task CreateOrder_DatabaseError_ReturnsInternalServerError()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();

            _mockService.Setup(s => s.CreateOrderAsync(orderDto))
                .ThrowsAsync(new DbUpdateException("Database error"));

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Internal server error");
        }

        [Test]
        public async Task CreateOrder_UnexpectedError_ReturnsInternalServerError()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();

            _mockService.Setup(s => s.CreateOrderAsync(orderDto))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Internal server error");
        }
    }
}
