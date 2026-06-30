using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.Model.Entities;
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

    [HttpGet("GetAllMilestones")]
    [Authorize(Roles = "Manager")]
    public IActionResult GetAllMilestones()
    {
        var userId = GetCurrentUserId();
        var milestones = _milestoneService.GetMilestonesForManager(userId);
        if (milestones == null)
            throw new BadRequestException("No Milestones Present");
        return Ok(milestones);
    }

    [HttpGet("GetMilestoneById/{Milestoneid}")]
    [Authorize(Roles = "Manager")]
    public IActionResult GetMilestoneById(int? Milestoneid)
    {
        if (Milestoneid == null)
            throw new BadRequestException("Enter the Milestone ID");

        var userId = GetCurrentUserId();
        var canAccess = _milestoneService.CanManagerAccessMilestone(userId, Milestoneid.Value);
        if (!canAccess)
            throw new BadRequestException("Cannot access this Milestone");

        var milestone = _milestoneService.GetById(Milestoneid.Value);
        if (milestone == null)
            return NotFound("Milestone not found");

        return Ok(milestone);
    }

    [HttpGet("GetByProjectId/{projectId}")]
    [Authorize(Roles = "Manager")]
    public IActionResult GetByProjectId(int? projectId)
    {
        if (projectId == null) throw new BadRequestException("Enter the Project Id");

        var userId = GetCurrentUserId();
        var canAccessProject = _milestoneService.CanManagerAccessProject(userId, projectId.Value);
        if (!canAccessProject)
            throw new BadRequestException("Cannot access this project");

        var milestones = _milestoneService.GetByProjectId(projectId.Value);
        if (milestones == null)
            throw new NotFoundException("Milestone not found");
        return Ok(milestones);
    }

    [HttpPost("CreateMilestone")]
    [Authorize(Roles = "Manager")]
    public IActionResult CreateMilestone(MilestoneCreateDTO dto)
    {
        if (dto == null) throw new BadRequestException("Enter the Required Details");

        var userId = GetCurrentUserId();
        var canAccessProject = _milestoneService.CanManagerAccessProject(userId, dto.ProjectId);
        if (!canAccessProject)
            throw new BadRequestException("No access for this project");

        var milestone = _milestoneService.Create(dto);
        if (milestone == null)
            throw new BadRequestException("Milestone couldn't be created");

        return Ok(milestone);
    }

    [HttpPost("UpdateMilestone/{Milestoneid}")]
    [Authorize(Roles = "Manager")]
    public IActionResult UpdateMilestone(int? Milestoneid, MilestoneUpdateDTO dto)
    {
        if (Milestoneid == null || dto == null) throw new BadRequestException("Enter both Id and required Details");

        var userId = GetCurrentUserId();
        var canAccessMilestone = _milestoneService.CanManagerAccessMilestone(userId, Milestoneid.Value);
        if (!canAccessMilestone)
            throw new BadRequestException("Cannot access this Milestone");

        var result = _milestoneService.Update(Milestoneid.Value, dto);
        if (!result)
            throw new NotFoundException("Milestone Couldn't be updated");

        return Ok("Milestone updated successfully");
    }

    private int GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var currentUserId))
            throw new UnauthorizedException("Invalid token");

        return currentUserId;
    }
}