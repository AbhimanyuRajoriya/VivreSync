using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.Allocations.DTOs;
using VivreSync.Allocations.Services;
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
            var role = GetCurrentRole();
            var allocations = _allocationService.GetAll(userId, role);

            return Ok(allocations);
        }

        [HttpGet("GetAllocationById/{allocationId}")]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult GetAllocationById(int allocationId)
        {
            if (allocationId <= 0)
                throw new BadRequestException("Enter valid allocation ID");

            var userId = GetCurrentUserId();
            var role = GetCurrentRole();
            var allocation = _allocationService.GetById(allocationId, userId, role);

            return Ok(allocation);
        }

        [HttpPost("CreateAllocation")]
        [Authorize(Roles = "Manager")]
        public IActionResult CreateAllocation(AllocationCreateDTO dto)
        {
            var userId = GetCurrentUserId();
            var result = _allocationService.Create(dto, userId);
            return Ok(result);
        }

        [HttpPost("UpdateAllocation/{allocationId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult UpdateAllocation(int allocationId, AllocationUpdateDTO dto)
        {
            if (allocationId <= 0)
                throw new BadRequestException("Enter valid allocation ID");

            var userId = GetCurrentUserId();
            var success = _allocationService.Update(allocationId, dto, userId);
            if (!success)
                throw new BadRequestException("Allocation could not be updated");

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

        [HttpPost("EndAllocation/{allocationId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult EndAllocation(int allocationId)
        {
            if (allocationId <= 0)
                throw new BadRequestException("Enter valid allocation ID");

            var userId = GetCurrentUserId();
            var result = _allocationService.EndAllocation(allocationId, userId);
            if (!result)
                throw new BadRequestException("Allocation could not be ended");

            return Ok("Allocation ended successfully");
        }
        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var currentUserId))
                throw new UnauthorizedException("Invalid token");

            return currentUserId;
        }

        private string GetCurrentRole()
        {
            if (User.IsInRole("Manager"))
                return "Manager";
            if (User.IsInRole("Employee"))
                return "Employee";

            throw new UnauthorizedException("Invalid role");
        }
    }
}