using AutoMapper;
using backend_amatista.Models;
using backend_amatista.Models.DTO;
using backendAmatista.Models.DTO;

namespace backend_amatista.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SaleDetail, SaleDetailDTO>()
                .ForMember(dest => dest.IdProduct, opt => opt.MapFrom(src => src.IdProduct))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(dest => dest.IdSale, opt => opt.MapFrom(src => src.IdSale));

            CreateMap<SaleDetailDTO, SaleDetail>()
                .ForMember(dest => dest.IdProduct, opt => opt.MapFrom(src => src.IdProduct))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(dest => dest.IdSale, opt => opt.MapFrom(src => src.IdSale));


            CreateMap<Sale, SaleDTO>()
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.InvoiceNumber))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Cuit, opt => opt.MapFrom(src => src.Cuit));



            CreateMap<SaleDTO, Sale>()
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.InvoiceNumber))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Cuit, opt => opt.MapFrom(src => src.Cuit));


        }
    }
}
