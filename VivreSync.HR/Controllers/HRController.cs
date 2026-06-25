using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.HR.DTOs;
using VivreSync.HR.Services;
using VivreSync.Model.Entities;
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
        var employees = _service.GetAll();
        if (employees == null) throw new BadRequestException("No Employees");
        return Ok(_service.GetAll());
    }

    [HttpGet("employees/{Employeeid}")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public IActionResult GetEmployeeById(int? Employeeid)
    {
        if (Employeeid == null) throw new BadRequestException("Enter the Employee ID");

        if (User.IsInRole("Employee"))
        {
            var currentUserId = GetCurrentUserId();
            var isOwnProfile = _service.IsEmployeeLinkedToUser(Employeeid.Value, currentUserId);
            if (!isOwnProfile)
                throw new UnauthorizedException("Cannot access this data");
        }

        var employee = _service.GetById(Employeeid.Value);
        if (employee == null)
            return NotFound("Employee not found");

        return Ok(employee);
    }

    [HttpPost("EmployeeAdd")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateEmployee(EmployeeCreateDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var employee = _service.Create(dto);
        if (employee == null)
            return BadRequest("Enter Valid Request");

        return Ok(employee);
    }

    [HttpPost("EmployeeUpdate")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateEmployee(EmployeeUpdateDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var result = _service.Update(dto);
        if (result == null)
            return NotFound("Employee not found");

        return Ok("Employee updated successfully");
    }

    [HttpPost("EmployeeDeactivate/{Employeeid}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeactiavteEmployee(int? Employeeid)
    {
        if (Employeeid == null)
            throw new BadRequestException("Enter the Employee ID");

        var result = _service.Deactivate(Employeeid);
        if (!result)
            return BadRequest("Not Found");
        return Ok("Employee Deactivated");
    }

    [HttpGet("Inactive_Employee")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetInactiveEmployees()
    {
        var result = _service.GetInactiveEmployees();
        if (result == null)
            return BadRequest("Not Found");
        return Ok(result);
    }

    [HttpPatch("activate/{Employeeid}")]
    [Authorize(Roles = "Admin")]
    public IActionResult ActivateEmployee(int? Employeeid)
    {
        if (Employeeid == null)
            throw new BadRequestException("Enter the Employee ID");

        var result = _service.ActivateEmployee(Employeeid);
        if (result == null)
            return BadRequest("Not Found");
        return Ok(result);
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
        var result = _skillService.GetAllSkills();
        if (result == null)
            throw new BadRequestException("No Skill Present");
        return Ok(_skillService.GetAllSkills());
    }

    [HttpPost("SkillsAdd")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateSkill(SkillCreateDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var result = _skillService.CreateSkill(dto);
        if(result == null)
            return BadRequest("Invalid Request");
        return Ok("Skill Created");
    }

    [HttpPost("SkillAssign")]
    [Authorize(Roles = "Admin")]
    public IActionResult AssignSkill(SkillAssignDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var success = _skillService.AssignSkillToEmployee(dto);
        if (!success)
            return BadRequest("Invalid Request");
        return Ok("Skill Assigned");
    }
}