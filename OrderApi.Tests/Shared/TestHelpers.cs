using AutoMapper;
using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OrderApi.Data;
using OrderApi.Data.DTOs;
using OrderApi.Data.DTOs.Input;
using OrderApi.Data.Profiles;
using OrderApi.Data.Repositories.Implementations;
using OrderApi.Data.Repositories.Interfaces;
using OrderApi.Services.Implementations;
using OrderApi.Services.Interfaces;

namespace OrderApi.Tests.Shared
{
    public static class TestHelpers
    {
        public static IMapper CreateTestMapper()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(cfg => cfg.AddProfile<OrderProfile>());
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IMapper>();
        }

        public static ILogger<T> CreateTestLogger<T>()
        {
            //return Mock.Of<ILogger<T>>();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            return loggerFactory.CreateLogger<T>();
        }

        public static AppDbContext CreateInMemoryDbContext(string dbName = "TestDb")
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static IOrderRepository CreateOrderRepository(AppDbContext context = null)
        {
            context ??= CreateInMemoryDbContext();
            var logger = CreateTestLogger<OrderRepository>();
            return new OrderRepository(context, logger);
        }

        public static IOrderService CreateOrderService(
            IOrderRepository repository = null,
            IMapper mapper = null)
        {
            repository ??= Mock.Of<IOrderRepository>();
            mapper ??= CreateTestMapper();
            var logger = CreateTestLogger<OrderService>();
            return new OrderService(repository, mapper, logger);
        }

        public static CreateOrderDto CreateValidOrderDto()
        {
            return new CreateOrderDto
            {
                CustomerName = "Test Customer",
                Items = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5
                }
            }
            };
        }

        public static IUrlHelper CreateMockUrlHelper(string actionUrl = null)
        {
            var urlHelper = new Mock<IUrlHelper>();

            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns(actionUrl ?? "https://localhost:5001/api/orders/12345");

            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("https://localhost:5001/api/orders/12345");

            return urlHelper.Object;
        }

        // NUnit 特定的断言扩展
        public static void ShouldBeEquivalentWithDetails(this object actual, object expected, string because = "")
        {
            actual.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering(), 
            because);
        }
    }
}
