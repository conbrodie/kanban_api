using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Repositories;
using api.Models;
using Microsoft.AspNetCore.JsonPatch;
using api.DTO;
using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ValidateModel]
    public class ProjectsController : ControllerBase
    {   
        private readonly IProjectsRepository  _projectsRepository;
        private IMapper _mapper;

        public ProjectsController(IProjectsRepository projectsRepository, IMapper mapper)
        {
            _projectsRepository = projectsRepository;
            _mapper = mapper;
        } 


        /// <summary>
        /// Gets projects that have been approved or not approved.
        /// </summary>
        /// <returns>A list of projects</returns>
        /// <response code="200">Returns the projects.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("approved/{status}")]
        public async Task<ActionResult<ICollection<ProjectPreviewDTO>>> GetProjectsByStatus([Required] bool status) 
        {
            var projects = await _projectsRepository.GetProjects(status);

            ICollection<ProjectPreviewDTO> projectPreviewDTO =  _mapper.Map<ICollection<Project>, ICollection<ProjectPreviewDTO>>(projects);

            return Ok(projectPreviewDTO);
        } 

        /// <summary>
        /// Gets projects previews - a summary of the project.
        /// </summary>
        /// <returns>A list of project previews.</returns>
        /// <response code="200">Returns the project previews.</response>
        [ProducesResponseType(200)]
        [HttpGet("previews")]
        public async Task<ActionResult<ICollection<ProjectPreviewDTO>>> GetProjectPreviews() 
        {
            var projectPreviews = await _projectsRepository.GetProjectPreviews();

            ICollection<ProjectPreviewDTO> projectPreviewDTO =  _mapper.Map<ICollection<Project>, ICollection<ProjectPreviewDTO>>(projectPreviews);

            return Ok(projectPreviewDTO);
        } 

        /// <summary>
        /// Gets the members within in a project.
        /// </summary>
        /// <returns>A list of members</returns>
        /// <response code="200">Returns the members</response>
        /// <response code="400">One or more validation errors occurred.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ValidateProjectExists]
        [HttpGet("{projectId}/members")]
        public async Task<ActionResult<ICollection<ProjectMemberDTO>>> GetProjectMembers([Required] int projectId) 
        {
            var projectMembers = await _projectsRepository.GetProjectMembers(projectId);

            ICollection<ProjectMemberDTO> projectMemberDTOs = _mapper.Map<ICollection<ProjectMember>, ICollection<ProjectMemberDTO>>(projectMembers);

            return Ok(projectMemberDTOs);
        } 

        /// <summary>
        /// Gets a project.
        /// </summary>
        /// <returns>A project</returns>
        /// <response code="200">Returns the project.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ValidateProjectExists]
        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDTO>> GetProject ([Required] int projectId) 
        {
            var project = await _projectsRepository.GetProject(projectId);

            var projectDTO = _mapper.Map<ProjectDTO>(project);

            return Ok(projectDTO);
        }

        /// <summary>
        /// Updates a project.
        /// </summary>
        /// <returns>No content.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     put /project
        ///     {
        ///        "ProjectId": "1",
        ///        "Title": "Project Title",
        ///        "StartDate": 2022-02-18T06:00:51.826Z,
        ///        "EndDate": 2022-02-18T06:00:51.826Z,
        ///        "Approved": false
        ///     }
        ///
        /// </remarks>
        /// <response code="204">Updated the project.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when updating the project.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateProjectExists]
        [ValidateUsersExists]
        [ValidateDepartmentsExists]
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(int projectId, [FromBody, Required] Project project,  [FromQuery, Required] List<int> userId, [FromQuery] List<int> departmentId)
        {
            if (projectId != project.ProjectId)
            {
                ModelState.AddModelError("", "ProjectId in request body does not match path Id.");
                return new BadRequestError(ModelState);
            }

            if (await _projectsRepository.UpdateProject(project, userId, departmentId) == false)
            {
               return new InternalServerError();
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a project.
        /// </summary>
        /// <returns>A created project.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /project
        ///     {
        ///        "Title": "Project Title",
        ///        "StartDate": 2022-02-18T06:00:51.826Z,
        ///        "EndDate": 2022-02-18T06:00:51.826Z,
        ///        "Approved": false
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Created the project.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="400">Not found.</response>
        /// <response code="500">Something went wrong when creating the project.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateUsersExists]
        [ValidateDepartmentsExists]
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject([FromBody, Required] Project project, [FromQuery, Required] List<int> userId, [FromQuery] List<int> departmentId)
        {
            if (!await _projectsRepository.CreateProject(project, userId, departmentId))
            {
                return new InternalServerError();
            }

            return CreatedAtAction(nameof(GetProject), new { ProjectId = project.ProjectId, }, project);
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Deleted the project.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when deleting the project.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateProjectExists]
        [HttpDelete("{projectId}")]
        public async Task<ActionResult<Project>> DeleteProject([Required] int projectId)
        {
            var project = await _projectsRepository.GetProject(projectId);

            if (await _projectsRepository.DeleteProject(project) == false)
            {
                return new InternalServerError();
            }

            return NoContent();
        }
    }
}

