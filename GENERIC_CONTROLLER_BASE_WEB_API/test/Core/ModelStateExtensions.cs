using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace test.Core
{
    /// <summary>
    /// Extension methods for ModelStateDictionary
    /// </summary>
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Extracts all validation errors from ModelState
        /// </summary>
        /// <param name="modelState">The ModelStateDictionary</param>
        /// <returns>A list of error messages</returns>
        public static List<string> GetValidationErrors(this ModelStateDictionary modelState)
        {
            return modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }
    }
} 