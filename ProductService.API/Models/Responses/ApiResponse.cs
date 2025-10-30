namespace ProductService.API.Models.Responses
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse(int statusCode, bool success, string message, T? data = default)
        {
            StatusCode = statusCode;
            Success = success;
            Message = message;
            Data = data;
        }
        //public ApiResponse(int code, string? msg = null)
        //{
        //    StatusCode = code;
        //    Message = msg ?? GetDefaultMsg(code);
        //}

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
            => new(statusCode, true, message, data);

        public static ApiResponse<T> FailResponse(List<string> errors, string message = "An error occurred", int statusCode = 400)
            => new(statusCode, false, message)
            {
                Errors = errors
            };


        //private string? GetDefaultMsg(int code)
        //{
        //    return code switch
        //    {
        //        400 => "Bad Request",
        //        401 => "UnAuthorized",
        //        404 => "Not Found",
        //        500 => "Internal Server Error",
        //        _ => null
        //    };
        //}

    }
}
