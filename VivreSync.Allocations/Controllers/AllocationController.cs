using Microsoft.AspNetCore.Mvc;
using VivreSync.Allocations.DTOs;
using VivreSync.Allocations.Services;

namespace VivreSync.Allocations.Controllers
{
    [ApiController]
    [Route("api/allocations")]
    public class AllocationsController : ControllerBase
    {
        private readonly IAllocationService _allocationService;

        public AllocationsController(IAllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var allocations = _allocationService.GetAll();
            return Ok(allocations);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var allocation = _allocationService.GetById(id);

            if (allocation == null)
                return NotFound("Allocation not found");

            return Ok(allocation);
        }

        [HttpPost("Create")]
        public IActionResult Create(AllocationCreateDTO dto)
        {
            var allocation = _allocationService.Create(dto);

            if (allocation == null)
                return BadRequest("Invalid allocation");

            return Ok(allocation);
        }

        [HttpPost("Update/{id}")]
        public IActionResult Update(int id, AllocationUpdateDTO dto)
        {
            var success = _allocationService.Update(id, dto);

            if (!success)
                return BadRequest("Invalid allocation");

            return Ok();
        }
    }
}