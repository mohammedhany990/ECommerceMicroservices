using Microsoft.AspNetCore.Mvc;
using ProductService.API.Models.Responses;

namespace ProductService.API.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            return code switch
            {
                400 => BadRequest(ApiResponse<object>.FailResponse(
                            new List<string> { "Bad Request" },
                            "The request could not be understood or was missing required parameters.",
                            400)),

                401 => Unauthorized(ApiResponse<object>.FailResponse(
                            new List<string> { "Unauthorized" },
                            "Authentication is required or has failed.",
                            401)),

                404 => NotFound(ApiResponse<object>.FailResponse(
                            new List<string> { "Not Found" },
                            "The requested resource was not found.",
                            404)),

                _ => StatusCode(code, ApiResponse<object>.FailResponse(
                            new List<string> { "Unexpected Error" },
                            "An unexpected error occurred.",
                            code))
            };
        }
    }

}
