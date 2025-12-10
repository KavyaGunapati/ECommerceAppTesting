using AutoMapper;
using EcommerceWebAPI.Models.DTOs;
using ECommerceWebAPI.DataAccess.Entities;

namespace ECommerceWebAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<OrderItem, OrderItemResponse>().ReverseMap();
            CreateMap<CreateOrder, Order>();
        }
    }
}
