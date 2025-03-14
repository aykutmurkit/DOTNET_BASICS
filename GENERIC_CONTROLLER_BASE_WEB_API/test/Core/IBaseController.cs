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
        Task<ActionResult> GetAll();
        Task<ActionResult> GetById(int id);
        Task<ActionResult> Create([FromBody] TCreateDto createDto);
        Task<ActionResult> Update(int id, [FromBody] TUpdateDto updateDto);
        Task<ActionResult> Delete(int id);
    }
} 