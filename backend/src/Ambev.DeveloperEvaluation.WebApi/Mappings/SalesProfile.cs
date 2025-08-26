using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Requests;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Responses;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    public class SalesProfile : Profile
    {
        public SalesProfile()
        {
            CreateMap<CreateSaleRequest, CreateSaleCommand>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CreateSaleItemRequest, CreateSaleItemDto>();

            CreateMap<GetSaleResult, SaleResponse>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<GetSaleItemDto, SaleItemResponse>();
        }
    }
}
