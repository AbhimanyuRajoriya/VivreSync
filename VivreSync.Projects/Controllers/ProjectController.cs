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
            if (User.IsInRole("Manager"))
            {
                var userId = GetCurrentUserId();
                var managerEmployeeId = _employeeService.GetEmployeeIdByUserId(userId);
                return Ok(_projectService.GetProjectsByManager(managerEmployeeId));
            }

            return Ok(_projectService.GetAll());
        }

        [HttpGet("GetProjectHealth/{projectId}")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetProjectHealth(int projectId)
        {
            if (projectId <= 0) throw new BadRequestException("Enter valid project ID");

            if (User.IsInRole("Manager"))
            {
                var userId = GetCurrentUserId();
                var managerEmployeeId = _employeeService.GetEmployeeIdByUserId(userId);
                var isOwnProject = _projectService.IsProjectManagedBy(projectId, managerEmployeeId);

                if (!isOwnProject)
                    throw new BadRequestException("Cannot access this project");
            }
            return Ok(_projectService.GetProjectHealth(projectId));
        }

        [HttpPost("CreateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateProject(ProjectCreateDTO dto)
        {
            if (dto == null) throw new BadRequestException("Enter the Required Details");

            var project = _projectService.Create(dto);
            if (project == null)
                throw new BadRequestException("Cannot create project with these details");

            return Ok(project);
        }

        [HttpPost("UpdateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProject(ProjectUpdateDTO dto)
        {
            if (dto == null) throw new BadRequestException("Enter the Details");

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
    }
}
