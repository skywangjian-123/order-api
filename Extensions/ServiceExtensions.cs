using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Data.Profiles;
using OrderApi.Data.Repositories.Implementations;
using OrderApi.Data.Repositories.Interfaces;
using OrderApi.Services.Implementations;
using OrderApi.Services.Interfaces;

namespace OrderApi.Extensions
{
    public static class ServiceExtensions
    {
        // 移除 SQLitePCL.Batteries.Init(); 因为未引用 SQLitePCL.Batteries 包且通常不需要手动初始化
        public static void ConfigureSqliteContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlite(config.GetConnectionString("DefaultConnection")));
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<OrderProfile>());
        }
    }
}
