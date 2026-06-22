using Microsoft.AspNetCore.Mvc;
using VivreSync.HR.Services;
using VivreSync.HR.DTOs;

namespace VivreSync.HR.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet("Employees")]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("employees/{id}")]
    public IActionResult GetEmployeeById(int id)
    {
        var employee = _service.GetById(id);

        if (employee == null)
            return NotFound("Employee not found");

        return Ok(employee);
    }

    [HttpPost("EmployeeAdd")]
    public IActionResult CreateEmployee(EmployeeCreateDTO dto)
    {
        var employee = _service.Create(dto);

        if (employee == null)
            return BadRequest("Enter Valid Request");

        return Ok(employee);
    }

    [HttpPost("EmployeeUpdate")]
    public IActionResult UpdateEmployee(EmployeeUpdateDTO dto)
    {
        var result = _service.Update(dto);

        if (result == null)
            return NotFound("Employee not found");

        return Ok("Employee updated successfully");
    }

    [HttpPost("EmployeeDeactivate/{id}")]
    public IActionResult DeactiavteEmployee(int id)
    {
        var result = _service.Deactivate(id);
        if (!result)
            return BadRequest("Not Found");
        return Ok();
    }
}

[ApiController]
[Route("api/skill")]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;
    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }
    [HttpGet("Skills")]
    public IActionResult GetAllSkills()
    {
        return Ok(_skillService.GetAllSkills());
    }

    [HttpPost("SkillsAdd")]
    public IActionResult CreateSkill(SkillCreateDTO dto)
    {
        var result = _skillService.CreateSkill(dto);
        if(result == null)
            return BadRequest("Invalid Request");
        return Ok("Skill Created");
    }

    [HttpPost("SkillAssign")]
    public IActionResult AssignSkill(SkillAssignDTO dto)
    {
        var success = _skillService.AssignSkillToEmployee(dto);
        if (!success)
            return BadRequest("Invalid Request");
        return Ok("Skill Assigned");
    }
}