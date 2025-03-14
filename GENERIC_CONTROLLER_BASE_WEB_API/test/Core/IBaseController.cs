using Microsoft.AspNetCore.Mvc;
using test.Entities;

namespace test.Core
{
    public interface IBaseController<T, TDto, TCreateDto, TUpdateDto>
        where T : BaseEntity
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<ActionResult<IEnumerable<TDto>>> GetAll();
        Task<ActionResult<TDto>> GetById(int id);
        Task<ActionResult<TDto>> Create([FromBody] TCreateDto createDto);
        Task<ActionResult<TDto>> Update(int id, [FromBody] TUpdateDto updateDto);
        Task<ActionResult> Delete(int id);
    }
} 