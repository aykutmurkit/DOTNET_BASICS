using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace test.Core
{
    /// <summary>
    /// Extension methods for ControllerBase to return Result objects
    /// </summary>
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// Returns an ActionResult with the appropriate status code based on the Result
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="result">The Result object</param>
        /// <returns>An ActionResult with the appropriate status code</returns>
        public static ActionResult ToActionResult(this ControllerBase controller, Result result)
        {
            return new ObjectResult(result)
            {
                StatusCode = result.StatusCode
            };
        }

        /// <summary>
        /// Returns an ActionResult with the appropriate status code based on the Result
        /// </summary>
        /// <typeparam name="T">Type of the data in the Result</typeparam>
        /// <param name="controller">The controller instance</param>
        /// <param name="result">The Result object</param>
        /// <returns>An ActionResult with the appropriate status code</returns>
        public static ActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
        {
            return new ObjectResult(result)
            {
                StatusCode = result.StatusCode
            };
        }

        /// <summary>
        /// Creates a successful Result with OK status
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="message">Success message</param>
        /// <returns>An ActionResult with OK status</returns>
        public static ActionResult Ok(this ControllerBase controller, string message = "Operation completed successfully")
        {
            var result = Result.Ok(message);
            return controller.ToActionResult(result);
        }

        /// <summary>
        /// Creates a successful Result with OK status and data
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="controller">The controller instance</param>
        /// <param name="data">Data to return</param>
        /// <param name="message">Success message</param>
        /// <returns>An ActionResult with OK status and data</returns>
        public static ActionResult Ok<T>(this ControllerBase controller, T data, string message = "Operation completed successfully")
        {
            var result = Result<T>.Ok(data, message);
            return controller.ToActionResult(result);
        }

        /// <summary>
        /// Creates a successful Result with Created status and data
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="controller">The controller instance</param>
        /// <param name="data">Data to return</param>
        /// <param name="message">Success message</param>
        /// <returns>An ActionResult with Created status and data</returns>
        public static ActionResult Created<T>(this ControllerBase controller, T data, string message = "Resource created successfully")
        {
            var result = Result<T>.Ok(data, message, (int)HttpStatusCode.Created);
            return controller.ToActionResult(result);
        }

        /// <summary>
        /// Creates a failed Result with NotFound status
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="message">Not found message</param>
        /// <returns>An ActionResult with NotFound status</returns>
        public static ActionResult NotFound(this ControllerBase controller, string message = "Resource not found")
        {
            var result = Result.NotFound(message);
            return controller.ToActionResult(result);
        }

        /// <summary>
        /// Creates a failed Result with BadRequest status
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="message">Error message</param>
        /// <param name="errors">List of detailed error messages</param>
        /// <returns>An ActionResult with BadRequest status</returns>
        public static ActionResult BadRequest(this ControllerBase controller, string message, List<string> errors = null)
        {
            var result = Result.Fail(message, errors);
            return controller.ToActionResult(result);
        }

        /// <summary>
        /// Creates a successful Result with NoContent status
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <returns>An ActionResult with NoContent status</returns>
        public static ActionResult NoContent(this ControllerBase controller)
        {
            var result = Result.Ok("Resource deleted successfully", (int)HttpStatusCode.NoContent);
            return controller.ToActionResult(result);
        }
    }
} 