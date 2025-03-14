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
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        public override async Task<ActionResult> GetAll()
        {
            return await base.GetAll();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> GetById(int id)
        {
            return await base.GetById(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public override async Task<ActionResult> Create([FromBody] object createDto)
        {
            return await base.Create(createDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> Update(int id, [FromBody] object updateDto)
        {
            return await base.Update(id, updateDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
} 