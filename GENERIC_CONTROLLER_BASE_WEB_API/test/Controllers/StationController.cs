using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Station Management")]
    public class StationController : BaseControllerNonGeneric
    {
        private readonly StationService _stationService;

        public StationController(IServiceNonGeneric service)
            : base(service)
        {
            _stationService = (StationService)service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public override async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            return await base.GetAll();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<object>> GetById(int id)
        {
            return await base.GetById(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<ActionResult<object>> Create([FromBody] object createDto)
        {
            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<object>> Update(int id, [FromBody] object updateDto)
        {
            return await base.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
} 