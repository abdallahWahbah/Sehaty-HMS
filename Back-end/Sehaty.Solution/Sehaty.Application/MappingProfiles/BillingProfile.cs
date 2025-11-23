
namespace Sehaty.Application.MappingProfiles
{
    public class BillingProfile : Profile
    {
        public BillingProfile()
        {
            CreateMap<Billing, BillingReadDto>().AfterMap((src, dest) =>
            {
                dest.Name = src.Patient.FirstName + " " + src.Patient.LastName;
            }).ReverseMap();

            CreateMap<BillingAddDto, Billing>()
             .ForMember(dest => dest.BillDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => BillingStatus.Pending))
             .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(_ => 0m))
             .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.TotalAmount))
             .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(_ => 0m))
             .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(_ => 0m))
             .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
             .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.paymentMethod));




            CreateMap<BillingUpdateDto, Billing>()
           .ForMember(dest => dest.TotalAmount,
               opt => opt.MapFrom(src => src.Subtotal + src.TaxAmount - src.DiscountAmount));



        }
    }
}
