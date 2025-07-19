using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Data.Models;
using OrderApi.Data.Repositories.Implementations;
using OrderApi.Tests.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Tests.Repositories
{
    [TestFixture]
    public class OrderRepositoryTests
    {
        private AppDbContext _context;
        private OrderRepository _repository;

        [SetUp]
        public void Setup()
        {
            _context = TestHelpers.CreateInMemoryDbContext();
            var logger = TestHelpers.CreateTestLogger<OrderRepository>();
            _repository = new OrderRepository(_context, logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void CreateOrderAsync_NoItems_ThrowsValidationException()
        {
            // Arrange
            var order = new Order
            {
                CustomerName = "Test Customer",
                Items = new List<OrderItem>()
            };

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() =>
                _repository.CreateOrderAsync(order));
        }

        [Test]
        public void CreateOrderAsync_InvalidQuantity_ThrowsValidationException()
        {
            // Arrange
            var order = new Order
            {
                CustomerName = "Test Customer",
                Items = new List<OrderItem>
            {
                new OrderItem { ProductId = Guid.NewGuid(), Quantity = 0 }
            }
            };

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() =>
                _repository.CreateOrderAsync(order));
        }
    }
}
