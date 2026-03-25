namespace VoucherService.Service.DTO;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    public ApiResponse(bool success, string message, T? data, int statusCode = 200)
    {
        Success = success;
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }

    public static ApiResponse<T> Ok(T? data, string message = "Success", int statusCode = 200)
        => new(true, message, data, statusCode);

    public static ApiResponse<T> Error(string message, int statusCode = 400, T? data = default)
        => new(false, message, data, statusCode);
}
