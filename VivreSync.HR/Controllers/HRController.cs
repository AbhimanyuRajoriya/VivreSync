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

    [HttpGet("GetAllEmployees")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult GetAllEmployees()
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentRole();
        var employees = _service.GetAll(userId, role);

        return Ok(employees);
    }

    [HttpGet("GetEmployeeById/{employeeId}")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public IActionResult GetEmployeeById(int employeeId)
    {
        if (employeeId <= 0)
            throw new BadRequestException("Enter valid Employee ID");

        var userId = GetCurrentUserId();
        var role = GetCurrentRole();
        var employee = _service.GetById(employeeId, userId, role);

        return Ok(employee);
    }

    [HttpPost("CreateEmployee")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateEmployee(EmployeeCreateDTO dto)
    {
        if (dto == null)
            throw new BadRequestException("Enter the required details");

        var employee = _service.Create(dto);
        return Ok("Employee Created");
    }

    [HttpPost("UpdateEmployee")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateEmployee(EmployeeUpdateDTO dto)
    {
        if (dto == null)
            throw new BadRequestException("Enter the required details");

        var result = _service.Update(dto);
        return Ok("Employee Updated");
    }

    [HttpPatch("DeactivateEmployee/{employeeId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeactivateEmployee(int employeeId)
    {
        if (employeeId <= 0)
            throw new BadRequestException("Enter valid Employee ID");

        var result = _service.Deactivate(employeeId);
        if (!result)
            throw new BadRequestException("Employee not found");

        return Ok("Employee deactivated");
    }

    [HttpGet("GetInactiveEmployees")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetInactiveEmployees()
    {
        var result = _service.GetInactiveEmployees();

        return Ok(result);
    }

    [HttpPatch("ActivateEmployee/{employeeId}")]
    [Authorize(Roles = "Admin")]
    public IActionResult ActivateEmployee(int employeeId)
    {
        if (employeeId <= 0)
            throw new BadRequestException("Enter valid Employee ID");

        var result = _service.ActivateEmployee(employeeId);
        return Ok(result);
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
        if (User.IsInRole("Admin"))
            return "Admin";
        if (User.IsInRole("Manager"))
            return "Manager";
        if (User.IsInRole("Employee"))
            return "Employee";
        throw new UnauthorizedException("Invalid role");
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
    [HttpGet("GetAllSkills")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetAllSkills()
    {
        var result = _skillService.GetAllSkills();
        if (result == null)
            throw new BadRequestException("No Skill Present");
        return Ok(_skillService.GetAllSkills());
    }

    [HttpPost("CreateSkill")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateSkill(SkillCreateDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var result = _skillService.CreateSkill(dto);
        if(result == null)
            throw new BadRequestException("Skill couldn't be created");
        return Ok("Skill Created");
    }

    [HttpPost("AssignSkill")]
    [Authorize(Roles = "Admin")]
    public IActionResult AssignSkill(SkillAssignDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var success = _skillService.AssignSkillToEmployee(dto);
        if (!success)
            throw new BadRequestException("Skill Couldn't be assigned");
        return Ok("Skill Assigned");
    }
}