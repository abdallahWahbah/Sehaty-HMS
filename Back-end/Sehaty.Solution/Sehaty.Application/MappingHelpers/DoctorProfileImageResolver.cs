using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sehaty.Application.Dtos.DoctorDtos;
using Sehaty.Core.Entites;

namespace Sehaty.Application.MappingHelpers
{
    public class DoctorProfileImageResolver<TDestination> : IValueResolver<Doctor, TDestination, string>
    {
        private readonly IConfiguration configuration;

        public DoctorProfileImageResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(Doctor source, TDestination destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source.ProfilePhoto))
            {
                return $"{configuration["SehatyBaseUlr"]}/images/doctors/{source.ProfilePhoto}";
            }

            return String.Empty;
        }
    }
}
