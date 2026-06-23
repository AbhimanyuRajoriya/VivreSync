using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivreSync.Allocations.DTOs;
using VivreSync.Allocations.Services;

namespace VivreSync.Allocations.Controllers
{
    [ApiController]
    [Route("api/allocations")]
    [Authorize]
    public class AllocationsController : ControllerBase
    {
        private readonly IAllocationService _allocationService;

        public AllocationsController(IAllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetAll()
        {
            var allocations = _allocationService.GetAll();
            return Ok(allocations);
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetById(int id)
        {
            var allocation = _allocationService.GetById(id);

            if (allocation == null)
                return NotFound("Allocation not found");

            return Ok(allocation);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Create(AllocationCreateDTO dto)
        {
            var allocation = _allocationService.Create(dto);

            if (allocation == null)
                return BadRequest("Invalid allocation");

            return Ok(allocation);
        }

        [HttpPost("Update/{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Update(int id, AllocationUpdateDTO dto)
        {
            var success = _allocationService.Update(id, dto);

            if (!success)
                return BadRequest("Invalid allocation");

            return Ok();
        }

        [HttpGet("GetAllEmployees")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetAllEmplyee()
        {
            var employees = _allocationService.GetEmployeeTable();
            return Ok(employees);
        }

        [HttpGet("FreeEmployees")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetFreeEmployees()
        {
            var employees = _allocationService.GetFreeEmployee();
            return Ok(employees);
        }

        [HttpGet("OccupiedEmployees")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetOccupiedEmployees()
        {
            var employees = _allocationService.GetOccupiedEmployee();
            return Ok(employees);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Delete(int id)
        {
            var success = _allocationService.Delete(id);

            if (!success)
                return NotFound("Allocation not found");

            return Ok();
        }
    }
}