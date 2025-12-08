using Sehaty.Application.Shared;

namespace Sehaty.APIs.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToApiResponse(this Result result)
        {
            return result.ErrorType switch
            {
                ErrorType.None =>
                    new OkResult(),

                ErrorType.NotFound =>
                    new NotFoundObjectResult(
                        new ApiResponse(404,result.Error)),

                ErrorType.Validation =>
                    new BadRequestObjectResult(
                        new ApiResponse(400,result.Error)),

                ErrorType.BadRequest =>
                    new BadRequestObjectResult(
                        new ApiResponse(400,result.Error)),

                ErrorType.Conflict =>
                    new ConflictObjectResult(
                        new ApiResponse(409,result.Error)),

                ErrorType.Forbidden =>
                    new ObjectResult(
                        new ApiResponse(403,result.Error))
                    { StatusCode = 403 },

                ErrorType.Unauthorized =>
                    new UnauthorizedObjectResult(
                        new ApiResponse(401,result.Error)),

                _ =>
                    new ObjectResult(
                        new ApiResponse(500,"Unexpected error"))
                    { StatusCode = 500 }
            };
        }

        public static ActionResult ToApiResponse<T>(this Result<T> result)
        {
            return result.IsSuccess
                ? new OkObjectResult(result.Data)
                : ((Result) result).ToApiResponse();
        }
    }

}
