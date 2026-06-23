using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Services;

namespace VivreSync.Projects.Controllers;
[ApiController]
[Route("api/milestones")]
[Authorize]
public class MilestonesController : ControllerBase
{
    private readonly IMilestoneService _milestoneService;

    public MilestonesController(IMilestoneService milestoneService)
    {
        _milestoneService = milestoneService;
    }

    [HttpGet("getAll")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetAll()
    {
        var milestones = _milestoneService.GetAll();
        return Ok(milestones);
    }

    [HttpGet("getById/{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetById(int id)
    {
        var milestone = _milestoneService.GetById(id);

        if (milestone == null)
            return NotFound("Milestone not found");

        return Ok(milestone);
    }

    [HttpGet("project/{projectId}")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetByProjectId(int projectId)
    {
        var milestones = _milestoneService.GetByProjectId(projectId);
        if (milestones == null)
            return BadRequest();
        return Ok(milestones);
    }

    [HttpPost("createMilestone")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult Create(MilestoneCreateDTO dto)
    {
        var milestone = _milestoneService.Create(dto);

        if (milestone == null)
            return BadRequest("Invalid project id");

        return Ok(milestone);
    }

    [HttpPost("UpdateMilestone/{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult Update(int id, MilestoneUpdateDTO dto)
    {
        var result = _milestoneService.Update(id, dto);

        if (!result)
            return NotFound("Milestone not found");

        return Ok("Milestone updated successfully");
    }
}