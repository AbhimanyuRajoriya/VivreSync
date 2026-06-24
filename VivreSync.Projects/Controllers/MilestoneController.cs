using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Services;
using VivreSync.Shared.Exceptions;

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
        if (milestones == null)
            throw new BadRequestException("No Milestones Present");
        return Ok(milestones);
    }

    [HttpGet("getById/{Milestoneid}")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetById(int? Milestoneid)
    {
        if (Milestoneid == null)
            throw new BadRequestException("Enter the Milestone ID");

        var milestone = _milestoneService.GetById(Milestoneid.Value);
        if (milestone == null)
            return NotFound("Milestone not found");

        return Ok(milestone);
    }

    [HttpGet("project/{projectId}")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult GetByProjectId(int? projectId)
    {
        if (projectId == null) throw new BadRequestException("Enter the Project Id");

        var milestones = _milestoneService.GetByProjectId(projectId.Value);
        if (milestones == null)
            return BadRequest();
        return Ok(milestones);
    }

    [HttpPost("createMilestone")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult Create(MilestoneCreateDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var milestone = _milestoneService.Create(dto);
        if (milestone == null)
            return BadRequest("Invalid project id");

        return Ok(milestone);
    }

    [HttpPost("UpdateMilestone/{Milestoneid}")]
    [Authorize(Roles = "Admin, Manager")]
    public IActionResult Update(int? Milestoneid, MilestoneUpdateDTO dto)
    {
        if (Milestoneid == null || dto == null) throw new BadRequestException("Enter both Id and required Details");

        var result = _milestoneService.Update(Milestoneid.Value, dto);
        if (!result)
            return NotFound("Milestone not found");

        return Ok("Milestone updated successfully");
    }
}