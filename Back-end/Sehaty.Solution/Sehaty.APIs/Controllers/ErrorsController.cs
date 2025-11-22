namespace Sehaty.APIs.Controllers
{
    [Route("error/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public IActionResult Error(int code)
        {
            var response = new ApiResponse(code);

            return code switch
            {
                400 => BadRequest(response),
                401 => Unauthorized(response),
                403 => Forbid(),
                404 => NotFound(response),
                405 => StatusCode(405, response),
                408 => StatusCode(408, response),
                409 => Conflict(response),
                422 => UnprocessableEntity(response),
                500 => StatusCode(500, response),
                502 => StatusCode(502, response),
                503 => StatusCode(503, response),
                504 => StatusCode(504, response),
                _ => StatusCode(code, response)
            };
        }
    }
}
