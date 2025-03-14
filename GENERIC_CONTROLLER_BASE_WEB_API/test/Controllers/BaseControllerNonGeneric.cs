using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.Entities;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseControllerNonGeneric : ControllerBase, IBaseControllerNonGeneric
    {
        protected readonly IServiceNonGeneric _service;

        protected BaseControllerNonGeneric(IServiceNonGeneric service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        public virtual async Task<ActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return this.Ok(items, "Items retrieved successfully");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return this.NotFound($"Item with ID {id} not found");

            return this.Ok(item, "Item retrieved successfully");
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> Create([FromBody] object createDto)
        {
            if (!ModelState.IsValid)
                return this.BadRequest("Invalid data", ModelState.GetValidationErrors());

            var item = await _service.CreateAsync(createDto);
            return this.Created(item, "Item created successfully");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> Update(int id, [FromBody] object updateDto)
        {
            if (id != ((dynamic)updateDto).Id)
                return this.BadRequest("ID mismatch between URL and body");

            if (!ModelState.IsValid)
                return this.BadRequest("Invalid data", ModelState.GetValidationErrors());

            if (!await _service.ExistsAsync(id))
                return this.NotFound($"Item with ID {id} not found");

            var item = await _service.UpdateAsync(updateDto);
            return this.Ok(item, "Item updated successfully");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> Delete(int id)
        {
            if (!await _service.ExistsAsync(id))
                return this.NotFound($"Item with ID {id} not found");

            await _service.DeleteAsync(id);
            return this.NoContent();
        }
    }
} 