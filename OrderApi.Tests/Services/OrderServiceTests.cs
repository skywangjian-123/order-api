using AutoMapper;
using FluentAssertions;
using Moq;
using OrderApi.Data.DTOs.Input;
using OrderApi.Data.Models;
using OrderApi.Data.Repositories.Interfaces;
using OrderApi.Services.Implementations;
using OrderApi.Services.Interfaces;
using OrderApi.Tests.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Tests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private IMapper _mapper;
        private IOrderService _service;
        private Mock<IOrderRepository> _mockRepo;

        [SetUp]
        public void Setup()
        {
            _mapper = TestHelpers.CreateTestMapper();
            _mockRepo = new Mock<IOrderRepository>();
            var logger = TestHelpers.CreateTestLogger<OrderService>();
            _service = new OrderService(_mockRepo.Object, _mapper, logger);
        }

        [Test]
        public async Task CreateOrderAsync_ValidInput_ReturnsOrderResponse()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            var order = _mapper.Map<Order>(orderDto);
            order.OrderId = Guid.NewGuid();

            _mockRepo.Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ReturnsAsync(order);

            // Act
            var result = await _service.CreateOrderAsync(orderDto);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(order.OrderId);
            result.CustomerName.Should().Be(orderDto.CustomerName);
            result.Items.Should().HaveCount(1);
            result.Items[0].ProductId.Should().Be(orderDto.Items[0].ProductId);
            result.Items[0].Quantity.Should().Be(5);
        }

        [Test]
        public void CreateOrderAsync_WhenQuantityIsZero_ThrowsValidationException()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            orderDto.Items[0].Quantity = 0;

            Assert.ThrowsAsync<ValidationException>(() =>
                _service.CreateOrderAsync(orderDto));
        }

        [Test]
        public void CreateOrderAsync_InvalidCustomerName_ThrowsValidationException()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            orderDto.CustomerName = "";

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() =>
                _service.CreateOrderAsync(orderDto));
        }

        [Test]
        public void CreateOrderAsync_NoItems_ThrowsValidationException()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            orderDto.Items = new List<OrderItemDto>();

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() =>
                _service.CreateOrderAsync(orderDto));
        }

        public void CreateOrderAsync_WhenNoProductId_ThrowsValidationException()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            orderDto.Items[0].ProductId = Guid.Empty;

            Assert.ThrowsAsync<ValidationException>(() =>
                _service.CreateOrderAsync(orderDto));
        }

        [Test]
        public void CreateOrderAsync_RepositoryException_PropagatesException()
        {
            // Arrange
            var orderDto = TestHelpers.CreateValidOrderDto();
            _mockRepo.Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() =>
                _service.CreateOrderAsync(orderDto));
        }
    }
}
