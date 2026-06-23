using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.HR.DTOs;
using VivreSync.HR.Services;
using VivreSync.Shared.Exceptions;

namespace VivreSync.HR.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet("Employees")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("employees/{id}")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public IActionResult GetEmployeeById(int id)
    {
        if (User.IsInRole("Employee"))
        {
            var currentUserId = GetCurrentUserId();
            var isOwnProfile = _service.IsEmployeeLinkedToUser(id, currentUserId);
            if (!isOwnProfile)
                throw new UnauthorizedException("Cannot access this data");
        }

        var employee = _service.GetById(id);
        if (employee == null)
            return NotFound("Employee not found");

        return Ok(employee);
    }

    [HttpPost("EmployeeAdd")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateEmployee(EmployeeCreateDTO dto)
    {
        var employee = _service.Create(dto);

        if (employee == null)
            return BadRequest("Enter Valid Request");

        return Ok(employee);
    }

    [HttpPost("EmployeeUpdate")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateEmployee(EmployeeUpdateDTO dto)
    {
        var result = _service.Update(dto);

        if (result == null)
            return NotFound("Employee not found");

        return Ok("Employee updated successfully");
    }

    [HttpPost("EmployeeDeactivate/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeactiavteEmployee(int id)
    {
        var result = _service.Deactivate(id);
        if (!result)
            return BadRequest("Not Found");
        return Ok();
    }

    private int GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var currentUserId))
            throw new UnauthorizedException("Invalid token");

        return currentUserId;
    }
}

[ApiController]
[Route("api/skill")]
[Authorize]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;
    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }
    [HttpGet("Skills")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetAllSkills()
    {
        return Ok(_skillService.GetAllSkills());
    }

    [HttpPost("SkillsAdd")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateSkill(SkillCreateDTO dto)
    {
        var result = _skillService.CreateSkill(dto);
        if(result == null)
            return BadRequest("Invalid Request");
        return Ok("Skill Created");
    }

    [HttpPost("SkillAssign")]
    [Authorize(Roles = "Admin")]
    public IActionResult AssignSkill(SkillAssignDTO dto)
    {
        var success = _skillService.AssignSkillToEmployee(dto);
        if (!success)
            return BadRequest("Invalid Request");
        return Ok("Skill Assigned");
    }
}