using Microsoft.AspNetCore.Mvc;
using test.Entities;

namespace test.Core
{
    public interface IBaseControllerNonGeneric
    {
        Task<ActionResult<IEnumerable<object>>> GetAll();
        Task<ActionResult<object>> GetById(int id);
        Task<ActionResult<object>> Create([FromBody] object createDto);
        Task<ActionResult<object>> Update(int id, [FromBody] object updateDto);
        Task<ActionResult> Delete(int id);
    }
} 