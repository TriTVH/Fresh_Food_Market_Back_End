using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.DTO
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(bool success, string message, T data, int statusCode = 200)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        public static ApiResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
        {
            return new ApiResponse<T>(true, message, data, statusCode);
        }

        public static ApiResponse<T> Error(T data, string message, int statusCode = 400)
        {
            return new ApiResponse<T>(false, message, data, statusCode);
        }
    }
}
