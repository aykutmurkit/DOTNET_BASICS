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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<object>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<object>> Create([FromBody] object createDto)
        {
            var item = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = ((dynamic)item).Id }, item);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<object>> Update(int id, [FromBody] object updateDto)
        {
            if (id != ((dynamic)updateDto).Id)
                return BadRequest();

            if (!await _service.ExistsAsync(id))
                return NotFound();

            var item = await _service.UpdateAsync(updateDto);
            return Ok(item);
        }

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