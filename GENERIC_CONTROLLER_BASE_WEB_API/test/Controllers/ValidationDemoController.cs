using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationDemoController : ControllerBase
    {
        private readonly IValidationService _validationService;

        public ValidationDemoController(IValidationService validationService)
        {
            _validationService = validationService;
        }

        /// <summary>
        /// Demo endpoint for generic validation
        /// </summary>
        [HttpPost("generic")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ValidateGeneric([FromBody] CreateStationDto dto)
        {
            // Manual validation using the generic validators
            await _validationService.ValidateAsync(dto, isGeneric: true);
            
            // If validation passes, return success
            return this.Ok(dto, "Validation passed for generic architecture");
        }

        /// <summary>
        /// Demo endpoint for non-generic validation
        /// </summary>
        [HttpPost("non-generic")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ValidateNonGeneric([FromBody] CreateStationDto dto)
        {
            // Manual validation using the non-generic validators
            await _validationService.ValidateAsync(dto, isGeneric: false);
            
            // If validation passes, return success
            return this.Ok(dto, "Validation passed for non-generic architecture");
        }
    }
} 