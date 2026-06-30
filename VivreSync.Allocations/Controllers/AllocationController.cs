using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.Allocations.DTOs;
using VivreSync.Allocations.Services;
using VivreSync.HR.Services;
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

        [HttpGet("GetAllAllocation")]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult GetAllAllocation()
        {
            var userId = GetCurrentUserId();
            if (User.IsInRole("Manager"))
            {
                var allocations = _allocationService.GetAllocationsForManager(userId);
                return Ok(allocations);
            }

            if (User.IsInRole("Employee"))
            {
                var allocations = _allocationService.GetAllocationsForEmployee(userId);
                return Ok(allocations);
            }
            return BadRequest("Access Denied");
        }

        [HttpGet("GetAllocationById/{allocationId}")]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult GetAllocationById(int allocationId)
        {
            if (allocationId <= 0)
                throw new BadRequestException("Enter valid allocation ID");

            var userId = GetCurrentUserId();
            var allocation = _allocationService.GetById(allocationId);
            if (allocation == null)
                return NotFound("Allocation not found");

            if (User.IsInRole("Manager"))
            {
                var canAccessAllocation = _allocationService.CanManagerAccessAllocation(userId, allocationId);
                if (!canAccessAllocation)
                    throw new BadRequestException("Cannot Allocate the project");
            }
            if (User.IsInRole("Employee"))
            {
                var employeeAllocations = _allocationService.GetAllocationsForEmployee(userId);
                var isOwnAllocation = employeeAllocations.Any(a => a.Id == allocationId);
                if (!isOwnAllocation)
                    throw new BadRequestException("Cannot check the allocation of other user");
            }
            return Ok(allocation);
        }

        [HttpPost("CreateAllocation")]
        [Authorize(Roles = "Manager")]
        public IActionResult CreateAllocation(AllocationCreateDTO dto)
        {
            var userId = GetCurrentUserId();
            var canAccessEmployee = _allocationService.CanManagerAccessEmployee(userId, dto.EmployeeId);
            var canAccessProject = _allocationService.CanManagerAccessProject(userId, dto.ProjectId);
            if (!canAccessEmployee || !canAccessProject)
                throw new BadRequestException("Cannot allocate the this project to this employee");

            var result = _allocationService.Create(dto);
            return Ok(result);
        }

        [HttpPost("UpdateAllocation/{allocationId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult UpdateAllocation(int allocationId, AllocationUpdateDTO dto)
        {
            if (allocationId <= 0)
                throw new BadRequestException("Enter valid allocation ID");

            var userId = GetCurrentUserId();
            var canAccessExistingAllocation = _allocationService.CanManagerAccessAllocation(userId, allocationId);
            var canAccessNewEmployee = _allocationService.CanManagerAccessEmployee(userId, dto.EmployeeId);
            var canAccessNewProject = _allocationService.CanManagerAccessProject(userId,dto.ProjectId);
            if (!canAccessExistingAllocation || !canAccessNewEmployee || !canAccessNewProject)
                throw new BadRequestException("Cannot Update the Allocation");

            var success = _allocationService.Update(allocationId, dto);
            if (!success)
                return BadRequest("Invalid allocation");

            return Ok("Allocation updated");
        }

        [HttpGet("GetAllEmployees")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetAllEmployees()
        {
            var userId = GetCurrentUserId();
            var employees = _allocationService.GetEmployeeTable(userId);
            return Ok(employees);
        }

        [HttpGet("FreeEmployees")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetFreeEmployees()
        {
            var userId = GetCurrentUserId();
            var employees = _allocationService.GetFreeEmployee(userId);
            return Ok(employees);
        }

        [HttpGet("OccupiedEmployees")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetOccupiedEmployees()
        {
            var userId = GetCurrentUserId();
            var employees = _allocationService.GetOccupiedEmployee(userId);
            return Ok(employees);
        }

        [HttpDelete("DeleteAllocation/{allocationId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult DeleteAllocation(int allocationId)
        {
            if (allocationId <= 0)
                throw new BadRequestException("Enter valid allocation ID");

            var userId = GetCurrentUserId();
            var canAccessAllocation = _allocationService.CanManagerAccessAllocation(userId, allocationId);
            if (!canAccessAllocation)
                return BadRequest("Cannot access this allocation");

            var success = _allocationService.Delete(allocationId);
            if (!success)
                return NotFound("Allocation not found");

            return Ok("Allocation removed");
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