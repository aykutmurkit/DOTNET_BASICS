namespace LogAPI.Entities.Models
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        
        public bool IsSuccess { get; set; }
        
        public T? Data { get; set; }
        
        public string Message { get; set; }
        
        public static ApiResponse<T> Success(T data, string message = null, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                IsSuccess = true,
                Data = data,
                Message = message ?? "İşlem başarıyla tamamlandı"
            };
        }
        
        public static ApiResponse<T> Fail(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                IsSuccess = false,
                Data = default,
                Message = message
            };
        }
    }
} 