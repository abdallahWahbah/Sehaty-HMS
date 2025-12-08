namespace Sehaty.APIs.Extensions
{
    public static class ErrorServicesExtension
    {
        public static IServiceCollection AddErrorServices(this IServiceCollection services)
        {
            #region Validation Error Configuration
            services.Configure<ApiBehaviorOptions>(cfg =>
            {
                cfg.InvalidModelStateResponseFactory = (context) =>
                {
                    var errors = context.ModelState
                    .Where(P => P.Value!.Errors.Count > 0)
                    .SelectMany(P => P.Value!.Errors)
                    .Select(E => E.ErrorMessage);

                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });
            #endregion

            return services;
        }
    }
}
