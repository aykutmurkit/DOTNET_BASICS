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

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreateDto createDto)
        {
            var item = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = ((dynamic)item).Id }, item);
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TUpdateDto updateDto)
        {
            if (id != ((dynamic)updateDto).Id)
                return BadRequest();

            if (!await _service.ExistsAsync(id))
                return NotFound();

            var item = await _service.UpdateAsync(updateDto);
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult> Delete(int id)
        {
            if (!await _service.ExistsAsync(id))
                return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
} 