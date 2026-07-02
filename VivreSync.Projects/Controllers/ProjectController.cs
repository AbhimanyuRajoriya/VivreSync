using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.HR.Services;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Services;
using VivreSync.Shared.Exceptions;

namespace VivreSync.Projects.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;

        public ProjectsController(IProjectService projectService, IEmployeeService employee)
        {
            _projectService = projectService;
            _employeeService = employee;
        }

        [HttpGet("GetAllProjects")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult GetAllProjects()
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentRole();
            var projects = _projectService.GetAll(userId, role);

            return Ok(projects);
        }

        [HttpGet("GetProjectHealth/{projectId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetProjectHealth(int projectId)
        {
            if (projectId <= 0)
                throw new BadRequestException("Enter valid project ID");

            var userId = GetCurrentUserId();
            var health = _projectService.GetProjectHealth(projectId, userId);

            return Ok(health);
        }

        [HttpPost("CreateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateProject(ProjectCreateDTO dto)
        {
            if (dto == null)
                throw new BadRequestException("Enter the required details");
            var project = _projectService.Create(dto);

            return Ok(project);
        }

        [HttpPost("UpdateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProject(ProjectUpdateDTO dto)
        {
            if (dto == null)
                throw new BadRequestException("Enter the details");

            var result = _projectService.Update(dto);
            if (!result)
                throw new BadRequestException("Cannot update project with these details");

            return Ok("Project updated successfully");
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
            throw new UnauthorizedException("Invalid role");
        }
    }
}
