namespace Sehaty.APIs.Extensions
{
    public static class AppSwaggerExtension
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            #region Swagger Setting

            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Sehaty",
                    Description = "Sehaty Medical Web API"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your valid token in the text input below . \r\n\r\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
                });

                swagger.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            #endregion

            return services;
        }
    }
}
