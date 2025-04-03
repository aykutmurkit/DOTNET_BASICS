using System.Collections.Generic;

namespace LogLib.Core.Utilities
{
    /// <summary>
    /// Standardized API response class
    /// </summary>
    public class LogApiResponse<T>
    {
        public T? Data { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }

        /// <summary>
        /// Creates a successful response
        /// </summary>
        public static LogApiResponse<T> Success(T? data, string? message = null)
        {
            return new LogApiResponse<T>
            {
                Data = data,
                Message = message,
                IsSuccess = true,
                StatusCode = 200
            };
        }

        /// <summary>
        /// Creates a response for a created resource
        /// </summary>
        public static LogApiResponse<T> Created(T? data, string? message = null)
        {
            return new LogApiResponse<T>
            {
                Data = data,
                Message = message ?? "Resource created successfully",
                IsSuccess = true,
                StatusCode = 201
            };
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        public static LogApiResponse<T> Error(string message, int statusCode = 400)
        {
            return new LogApiResponse<T>
            {
                Message = message,
                IsSuccess = false,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a validation error response
        /// </summary>
        public static LogApiResponse<T> Error(Dictionary<string, List<string>> errors, string message = "Validation errors occurred", int statusCode = 400)
        {
            return new LogApiResponse<T>
            {
                Errors = errors,
                Message = message,
                IsSuccess = false,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a server error response
        /// </summary>
        public static LogApiResponse<T> ServerError(string message = "An internal server error occurred")
        {
            return new LogApiResponse<T>
            {
                Message = message,
                IsSuccess = false,
                StatusCode = 500
            };
        }
    }
} 