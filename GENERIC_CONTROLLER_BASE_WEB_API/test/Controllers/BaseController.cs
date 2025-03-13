using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.Entities;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T, TDto, TCreateDto, TUpdateDto> : ControllerBase
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get item by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// Create a new item
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto createDto)
        {
            var item = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = ((dynamic)item).Id }, item);
        }

        /// <summary>
        /// Update an existing item
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TUpdateDto updateDto)
        {
            if (id != ((dynamic)updateDto).Id)
                return BadRequest();

            if (!await _service.ExistsAsync(id))
                return NotFound();

            var item = await _service.UpdateAsync(updateDto);
            return Ok(item);
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> Delete(int id)
        {
            if (!await _service.ExistsAsync(id))
                return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
} 