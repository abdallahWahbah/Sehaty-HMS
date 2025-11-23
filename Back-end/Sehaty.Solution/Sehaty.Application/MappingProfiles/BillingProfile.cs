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
            .ForMember(dest => dest.BillDate, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => BillingStatus.Pending))
            .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(_ => 0m))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Subtotal + src.TaxAmount - src.DiscountAmount));





            CreateMap<BillingUpdateDto, Billing>()
           .ForMember(dest => dest.TotalAmount,
               opt => opt.MapFrom(src => src.Subtotal + src.TaxAmount - src.DiscountAmount));



        }
    }
}
