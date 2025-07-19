using AutoMapper;
using OrderApi.Data.DTOs.Input;
using OrderApi.Data.DTOs.Output;
using OrderApi.Data.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderApi.Data.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<CreateOrderDto, Order>()
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore());
            CreateMap<Order, OrderResponseDto>();
            CreateMap<OrderItem, OrderItemResponseDto>();
        }
    }
}
