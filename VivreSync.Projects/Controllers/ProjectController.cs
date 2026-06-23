using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Services;

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
            return Ok(projects);
        }

        [HttpGet("ProjectHealth/{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetProjectHealth(int id)
        {
            var result = _projectService.GetProjectHealth(id);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }

        [HttpPost("CreateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ProjectCreateDTO dto)
        {
            var project = _projectService.Create(dto);

            if (project == null)
                return BadRequest("Invalid manager id");

            return Ok(project);
        }

        [HttpPost("UpdateProject")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(ProjectUpdateDTO dto)
        {
            var result = _projectService.Update(dto);

            if (!result)
                return BadRequest("Project not found or invalid manager id");

            return Ok("Project updated successfully");
        }
    }
}
