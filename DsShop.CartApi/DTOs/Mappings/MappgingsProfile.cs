using AutoMapper;
using DsShop.CartApi.Models;

namespace DsShop.CartApi.DTOs.Mappings;

public class MappgingsProfile : Profile
{
    public MappgingsProfile()
    {
        CreateMap<CartDTO, Cart>().ReverseMap();
        CreateMap<CartHeaderDTO, CartHeader>().ReverseMap();
        CreateMap<CartItemDTO, CartItem>().ReverseMap();
        CreateMap<ProductDTO, Product>().ReverseMap();
    }
}
