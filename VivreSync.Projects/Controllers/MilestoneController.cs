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
        var milestones = _milestoneService.GetAll(userId);

        return Ok(milestones);
    }

    [HttpGet("GetMilestoneById/{milestoneId}")]
    [Authorize(Roles = "Manager")]
    public IActionResult GetMilestoneById(int milestoneId)
    {
        if (milestoneId <= 0)
            throw new BadRequestException("Enter valid Milestone ID");
        var userId = GetCurrentUserId();
        var milestone = _milestoneService.GetById(milestoneId, userId);

        return Ok(milestone);
    }

    [HttpGet("GetByProjectId/{projectId}")]
    [Authorize(Roles = "Manager")]
    public IActionResult GetByProjectId(int projectId)
    {
        if (projectId <= 0)
            throw new BadRequestException("Enter valid Project ID");
        var userId = GetCurrentUserId();
        var milestones = _milestoneService.GetByProjectId(projectId, userId);

        return Ok(milestones);
    }

    [HttpPost("CreateMilestone")]
    [Authorize(Roles = "Manager")]
    public IActionResult CreateMilestone(MilestoneCreateDTO dto)
    {
        if (dto == null)
            throw new BadRequestException("Enter the required details");
        var userId = GetCurrentUserId();
        var milestone = _milestoneService.Create(dto, userId);

        return Ok(milestone);
    }

    [HttpPost("UpdateMilestone/{milestoneId}")]
    [Authorize(Roles = "Manager")]
    public IActionResult UpdateMilestone(int milestoneId, MilestoneUpdateDTO dto)
    {
        if (milestoneId <= 0)
            throw new BadRequestException("Enter valid Milestone ID");
        if (dto == null)
            throw new BadRequestException("Enter the required details");

        var userId = GetCurrentUserId();
        var result = _milestoneService.Update(milestoneId, dto, userId);
        if (!result)
            throw new BadRequestException("Milestone could not be updated");

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