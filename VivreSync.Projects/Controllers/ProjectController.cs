using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var projects = _projectService.GetAll();
            if (projects == null)
                throw new BadRequestException("No Projects Added");
            return Ok(projects);
        }

        [HttpGet("ProjectHealth/{Projectid}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetProjectHealth(int? Projectid)
        {
            if (Projectid == null) throw new BadRequestException("Enter the Project Id");

            var result = _projectService.GetProjectHealth(Projectid.Value);
            if (result == null)
                return BadRequest("No Such Project");
            return Ok(result);
        }

        [HttpPost("CreateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ProjectCreateDTO dto)
        {
            if (dto == null) throw new BadRequestException("Enter the Required Details");

            var project = _projectService.Create(dto);
            if (project == null)
                return BadRequest("Couldn't Create Project With these Details");

            return Ok(project);
        }

        [HttpPost("UpdateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(ProjectUpdateDTO dto)
        {
            if (dto == null) throw new BadRequestException("Enter the Details");

            var result = _projectService.Update(dto);
            if (!result)
                return BadRequest("Couldn't Update Project Check Entered Data");

            return Ok("Project updated successfully");
        }
    }
}
