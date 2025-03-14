using System.Collections.Generic;
using System.Net;

namespace test.Core
{
    /// <summary>
    /// Base Result class for API responses
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result of the operation
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// List of error messages if operation failed
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// HTTP status code for the response
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Creates a successful result
        /// </summary>
        /// <param name="message">Success message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A successful Result</returns>
        public static Result Ok(string message = "Operation completed successfully", int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a failed result
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">List of detailed error messages</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A failed Result</returns>
        public static Result Fail(string message, List<string> errors = null, int statusCode = (int)HttpStatusCode.BadRequest)
        {
            return new Result
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a not found result
        /// </summary>
        /// <param name="message">Not found message</param>
        /// <returns>A not found Result</returns>
        public static Result NotFound(string message = "Resource not found")
        {
            return new Result
            {
                Success = false,
                Message = message,
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }
    }

    /// <summary>
    /// Generic Result class that includes data
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// Data returned by the operation
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Creates a successful result with data
        /// </summary>
        /// <param name="data">Data to return</param>
        /// <param name="message">Success message</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A successful Result with data</returns>
        public static Result<T> Ok(T data, string message = "Operation completed successfully", int statusCode = (int)HttpStatusCode.OK)
        {
            return new Result<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a failed result
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">List of detailed error messages</param>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>A failed Result</returns>
        public new static Result<T> Fail(string message, List<string> errors = null, int statusCode = (int)HttpStatusCode.BadRequest)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a not found result
        /// </summary>
        /// <param name="message">Not found message</param>
        /// <returns>A not found Result</returns>
        public new static Result<T> NotFound(string message = "Resource not found")
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }
    }
} 