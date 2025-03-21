using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.Entities;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T, TDto, TCreateDto, TUpdateDto> : ControllerBase, IBaseController<T, TDto, TCreateDto, TUpdateDto>
        where T : BaseEntity
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IService<T, TDto, TCreateDto, TUpdateDto> _service;

        protected BaseController(IService<T, TDto, TCreateDto, TUpdateDto> service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all items
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        public virtual async Task<ActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return this.Ok(items, "Items retrieved successfully");
        }

        /// <summary>
        /// Get item by id
        /// </summary>
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

        /// <summary>
        /// Create a new item
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> Create([FromBody] TCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return this.BadRequest("Invalid data", ModelState.GetValidationErrors());

            var item = await _service.CreateAsync(createDto);
            return this.Created(item, "Item created successfully");
        }

        /// <summary>
        /// Update an existing item
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> Update(int id, [FromBody] TUpdateDto updateDto)
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

        /// <summary>
        /// Delete an item
        /// </summary>
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