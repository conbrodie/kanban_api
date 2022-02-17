using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Repositories;
using api.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using api.DTO;
using System.ComponentModel.DataAnnotations;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {   
        private readonly IProjectsRepository  _projectsRepository;

        public ProjectsController(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        } 

        // [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("approved/{status}")]
        public async Task<ICollection<Project>> GetProjects([Required] bool status) 
        {
            return await _projectsRepository.GetProjects(status);
        } 

        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("previews")]
        public async Task<ICollection<ProjectPreviewDTO>> GetProjectPreviews() 
        {
            return await _projectsRepository.GetProjectPreviews();
        } 

        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("{projectId}/members")]
        public async Task<ICollection<Project>> GetProjectMembers([Required] int projectId) 
        {
            return await _projectsRepository.GetProjectMembers(projectId);
        } 

        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("{ProjectId}")]
        public async Task<ActionResult<Project>> GetProject ([Required] int ProjectId) 
        {
            var project = await _projectsRepository.GetProject(ProjectId);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPatch("{ProjectId}")]
        public async Task<IActionResult> PatchProject (int ProjectId, [FromBody] JsonPatchDocument<Project> patchDoc)
        {
            if (patchDoc != null)
            {
                var project = _projectsRepository.GetProject(ProjectId).Result;

                if (project == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!await _projectsRepository.PatchProject(project, patchDoc))
                {
                    ModelState.AddModelError("", $"Something went wrong when updating {project.Title}");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{ProjectId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProject([FromBody] Project project, int ProjectId,  [FromQuery] List<int> userId, [FromQuery] List<int> departmentId)
        {

            if (project == null)
            {
                return BadRequest(ModelState);
            }

            if (ProjectId != project.ProjectId)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _projectsRepository.UpdateProject(project, userId, departmentId) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when updating {project.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Project>> CreateProject([FromBody] Project project, [FromQuery, Required] List<int> userId, [FromQuery] List<int> departmentId)
        {
            if (project == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _projectsRepository.CreateProject(project, userId, departmentId))
            {
                ModelState.AddModelError("", $"Something went wrong when saving {project.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetProject), new { ProjectId = project.ProjectId, }, project);
        }

        [HttpDelete("{ProjectId}")]
        [Authorize(Policy = "IsProjectCreator")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Project>> DeleteProject([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _projectsRepository.DeleteProject(project) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when deleting {project.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

