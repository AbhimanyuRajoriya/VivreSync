using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.Allocations.DTOs;
using VivreSync.Allocations.Services;
using VivreSync.Model.Entities;
using VivreSync.Shared.Exceptions;

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
            if (allocations == null)
                throw new BadRequestException("No Allocations");
            return Ok(allocations);
        }

        [HttpGet("GetById/{Allocationid}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetById(int? Allocationid)
        {
            if (Allocationid == null) throw new BadRequestException("Enter The Allocation ID");

            var allocation = _allocationService.GetById(Allocationid.Value);
            if (allocation == null)
                return NotFound("Allocation not found");

            return Ok(allocation);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Create(AllocationCreateDTO dto)
        {
            if (dto == null) throw new BadRequestException("Enter the reqiured Data");

            if (User.IsInRole("Manager"))
            {
                var currentUserId = GetCurrentUserId();
                var canAccessProject = _allocationService.CanManagerAccessProject(currentUserId, dto.ProjectId);
                if (!canAccessProject)
                    throw new BadRequestException("Cannot create allocation for this project");
            }

            var allocation = _allocationService.Create(dto);
            if (allocation == null)
                return BadRequest("Invalid allocation");

            return Ok(allocation);
        }

        [HttpPost("Update/{Allocationid}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Update(int? Allocationid, AllocationUpdateDTO dto)
        {
            if (Allocationid == null || dto == null)
                throw new BadRequestException("Enter both Allocation Id and reqiured Data");

            if (User.IsInRole("Manager"))
            {
                var currentUserId = GetCurrentUserId();

                var canAccessExistingAllocation = _allocationService.CanManagerAccessAllocation(currentUserId, Allocationid.Value);
                if (!canAccessExistingAllocation)
                    throw new BadRequestException("Cannot access requested allocation");

                var canAccessNewProject = _allocationService.CanManagerAccessProject(currentUserId,dto.ProjectId);

                if (!canAccessNewProject)
                    throw new BadRequestException("Cannot access this project");
            }

            var success = _allocationService.Update(Allocationid.Value, dto);
            if (!success)
                return BadRequest("Invalid allocation");

            return Ok("Allocation Updated");
        }

        [HttpGet("GetAllEmployees")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetAllEmployees()
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

        [HttpDelete("Delete/{Allocationid}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Delete(int? Allocationid)
        {
            if (Allocationid == null) throw new BadRequestException("Enter the Allocation ID");

            if (User.IsInRole("Manager"))
            {
                var currentUserId = GetCurrentUserId();

                var canAccessExistingAllocation = _allocationService.CanManagerAccessAllocation(currentUserId, Allocationid.Value);
                if (!canAccessExistingAllocation)
                    return Forbid();
            }

            var success = _allocationService.Delete(Allocationid.Value);
            if (!success)
                return NotFound("Allocation not found");

            return Ok("Allocation Removed");
        }
        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var currentUserId))
                throw new UnauthorizedException("Invalid token");

            return currentUserId;
        }
    }
}