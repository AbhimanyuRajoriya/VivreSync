using Microsoft.AspNetCore.Mvc;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Services;

namespace VivreSync.Projects.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            var projects = _projectService.GetAll();
            return Ok(projects);
        }

        [HttpGet("ProjectHealth")]
        public IActionResult GetProjectHealth(int id)
        {
            var result = _projectService.GetProjectHealth(id);
            return Ok(result);
        }

        [HttpPost("CreateProject")]
        public IActionResult Create(ProjectCreateDTO dto)
        {
            var project = _projectService.Create(dto);

            if (project == null)
                return BadRequest("Invalid manager id");

            return Ok(project);
        }

        [HttpPost("UpdateProject/{id}")]
        public IActionResult Update(int id, ProjectUpdateDTO dto)
        {
            dto.Id = id;
            var result = _projectService.Update(dto);

            if (!result)
                return BadRequest("Project not found or invalid manager id");

            return Ok("Project updated successfully");
        }
    }
}
