using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using test.Core;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultDemoNonGenericController : ControllerBase
    {
        private readonly IServiceNonGeneric _service;

        public ResultDemoNonGenericController(IServiceNonGeneric service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all items with Result pattern
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<IEnumerable<object>>), 200)]
        public async Task<ActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return this.Ok(items, "Items retrieved successfully");
        }

        /// <summary>
        /// Get item by id with Result pattern
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<object>), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<ActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return this.NotFound($"Item with ID {id} not found");

            return this.Ok(item, "Item retrieved successfully");
        }

        /// <summary>
        /// Create a new item with Result pattern
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<object>), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<ActionResult> Create([FromBody] object createDto)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest("Invalid data", ModelState.GetValidationErrors());
            }

            var item = await _service.CreateAsync(createDto);
            return this.Created(item, "Item created successfully");
        }

        /// <summary>
        /// Update an existing item with Result pattern
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<object>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<ActionResult> Update(int id, [FromBody] object updateDto)
        {
            if (id != ((dynamic)updateDto).Id)
                return this.BadRequest("ID mismatch between URL and body");

            if (!ModelState.IsValid)
            {
                return this.BadRequest("Invalid data", ModelState.GetValidationErrors());
            }

            if (!await _service.ExistsAsync(id))
                return this.NotFound($"Item with ID {id} not found");

            var item = await _service.UpdateAsync(updateDto);
            return this.Ok(item, "Item updated successfully");
        }

        /// <summary>
        /// Delete an item with Result pattern
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result), 204)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<ActionResult> Delete(int id)
        {
            if (!await _service.ExistsAsync(id))
                return this.NotFound($"Item with ID {id} not found");

            await _service.DeleteAsync(id);
            return this.NoContent();
        }
    }
} 