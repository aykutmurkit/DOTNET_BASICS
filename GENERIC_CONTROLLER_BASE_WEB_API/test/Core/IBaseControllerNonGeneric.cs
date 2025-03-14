using Microsoft.AspNetCore.Mvc;
using test.Entities;

namespace test.Core
{
    public interface IBaseControllerNonGeneric
    {
        Task<ActionResult> GetAll();
        Task<ActionResult> GetById(int id);
        Task<ActionResult> Create([FromBody] object createDto);
        Task<ActionResult> Update(int id, [FromBody] object updateDto);
        Task<ActionResult> Delete(int id);
    }
} 