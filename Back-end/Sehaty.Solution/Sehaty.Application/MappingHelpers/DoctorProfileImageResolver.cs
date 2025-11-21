using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Core.Entites;

namespace Sehaty.Application.MappingHelpers
{
    public class DoctorProfileImageResolver : IValueResolver<Doctor, GetDoctorDto, string>
    {
        private readonly IConfiguration configuration;

        public DoctorProfileImageResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(Doctor source, GetDoctorDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source.ProfilePhotoUrl))
            {
                destination.ProfilePhotoUrl = $"{configuration["SehatyBaseUlr"]}/images/doctors/{source.ProfilePhotoUrl}";
                return destination.ProfilePhotoUrl;
            }

            return String.Empty;
        }
    }
}
