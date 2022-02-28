using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Repositories;
using api.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using api.DTO;
using AutoMapper;

namespace api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class SprintsController : ControllerBase
    {   
        private readonly IProjectsRepository  _projectsRepository;
        private readonly ISprintsRepository _sprintRepository;
        private readonly ISprintListRepository _sprintListRepository;
        private IMapper _mapper;

        public SprintsController(IProjectsRepository projectsRepository, ISprintsRepository sprintRepository, ISprintListRepository sprintListRepository, IMapper mapper)
        {
            _projectsRepository = projectsRepository;
            _sprintRepository = sprintRepository;
            _sprintListRepository = sprintListRepository;
            _mapper = mapper;
        } 

        /// <summary>
        /// Gets a project sprint.
        /// </summary>
        /// <returns>A project sprints.</returns>
        /// <response code="200">Returns a project sprint.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{ProjectId}/sprints/{sprintId}")]
        public async Task<ActionResult<SprintDTO>> GetProjectSprint ([Required] int ProjectId, [Required] int sprintId) 
        {
            var project = await _projectsRepository.GetProject(ProjectId);

            if (project == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Project not found.");
            }

            var sprint = await _sprintRepository.GetSprint(sprintId);

            if (sprint == null) 
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint not found.");  
            }

            var sprintDTO = _mapper.Map<SprintDTO>(sprint);

            return Ok(sprintDTO);
        }

        /// <summary>
        /// Gets all sprints for a given project.
        /// </summary>
        /// <returns>A list of project sprints.</returns>
        /// <response code="200">Returns a list of project sprints.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{ProjectId}/sprints")]
        public async Task<ActionResult<ICollection<SprintDTO>>> GetProjectSprints ([Required] int ProjectId) 
        {
            var project = await _projectsRepository.GetProject(ProjectId);

            if (project == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Project not found.");
            }

            var sprints = await _sprintRepository.GetSprintsForProject(ProjectId);

            ICollection<SprintDTO> sprintDTOs = _mapper.Map<ICollection<Sprint>, ICollection<SprintDTO>>(sprints);

            return Ok(sprintDTOs);
        }

        /// <summary>
        /// Gets a sprint list.
        /// </summary>
        /// <returns>A list sprint list.</returns>
        /// <response code="200">Returns a sprint list.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{ProjectId}/sprints/{SprintId}/lists/{SprintListId}")]
        public async Task<ActionResult<SprintListDTO>> GetProjectSprintList ([Required] int SprintId, [Required] int SprintListId) 
        {
            var sprint = await _sprintRepository.GetSprint(SprintId);

            if (sprint == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint not found.");
            }

            var list = await _sprintListRepository.GetSprintList(SprintListId);

             if (list == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint list not found.");
            }

            var sprintListDTO = _mapper.Map<SprintListDTO>(list);

            return Ok(sprintListDTO);
        }

        /// <summary>
        /// Gets all lists for a given sprint.
        /// </summary>
        /// <returns>A list of sprint lists.</returns>
        /// <response code="200">Returns a list of sprint lists.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{ProjectId}/sprints/{SprintId}/lists")]
        public async Task<ActionResult<ICollection<SprintList>>> GetProjectSprintLists ([Required] int SprintId) 
        {
            var sprint = await _sprintRepository.GetSprint(SprintId);

            if (sprint == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint not found.");
            }

            var lists = await _sprintListRepository.GetListsForSprint(SprintId);

            return Ok(lists);
        }

         /// <summary>
        /// Creates a project sprint.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /projects/{projectId}/sprints
        ///     {
        ///        "ProjectId": 1,
        ///        "SprintName": "Name",
        ///     }
        ///
        /// </remarks>
        /// <returns>A created project sprint.</returns>
        /// <response code="201">Creates a project sprint.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when creating the sprint.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpPost("{ProjectId}/sprints")]
        public async Task<ActionResult> CreateProjectSprint ([Required] int ProjectId, [FromBody, Required] Sprint sprint) 
        {
            var project = await _projectsRepository.GetProject(ProjectId);

            if (project == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Project not found.");
            }

            if (sprint == null) 
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _sprintRepository.CreateSprint(sprint) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when creating {sprint.SprintName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetProjectSprint), new { SprintId = sprint.SprintId, }, sprint);
        }

        /// <summary>
        /// Creates a project sprint list.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /projects/{projectId}/sprints/{SprintId}
        ///     {
        ///        "ProjectId": 1,
        ///        "SprintName": "Name",
        ///     }
        ///
        /// </remarks>
        /// <returns>A created project sprint.</returns>
        /// <response code="201">Returns a list of project sprints.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when creating the sprint list.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpPost("{ProjectId}/sprints/{SprintId}/lists")]
        public async Task<ActionResult> CreateProjectSprintList ([Required] int ProjectId, [Required] int SprintId, [FromBody, Required] SprintList sprintList) 
        {
            var project = await _projectsRepository.GetProject(ProjectId);
            var sprint = await _sprintRepository.GetSprint(SprintId);

            if (project == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Project not found.");
            }

            if (sprint == null) 
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _sprintListRepository.CreateSprintList(sprintList) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when creating {sprintList.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetProjectSprintList), new { SprintListId = sprintList.SprintListId, }, sprintList);
        }

        /// <summary>
        /// Deletes a project sprint.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Deleted the project sprint.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when deleting the project sprint.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpDelete("{ProjectId}/sprints")]
        public async Task<ActionResult<Project>> DeleteProjectSprint([Required] int projectId, [FromQuery, Required] int sprintId)
        {
            var project = await _projectsRepository.GetProject(projectId);

            if (project == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Project not found.");
            }

            var sprint = await _sprintRepository.GetSprint(sprintId);

            if (sprint == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint not found.");
            }

            if (await _sprintRepository.DeleteSprint(sprint) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when deleting {sprint.SprintName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

         /// <summary>
        /// Deletes a project sprint list.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Deleted the project sprint.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when deleting the project sprint.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpDelete("{projectId}/sprints/{sprintId}/lists")]
        public async Task<ActionResult<Project>> DeleteProjectSprintList([Required] int projectId, [Required] int sprintId, [FromQuery, Required] int sprintListId)
        {
            var project = await _projectsRepository.GetProject(projectId);

            if (project == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Project not found.");
            }

            var sprint = await _sprintRepository.GetSprint(sprintId);

            if (sprint == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint not found.");
            }

            var sprintList = await _sprintListRepository.GetSprintList(sprintListId);

            if (sprintList == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint list not found.");
            }

            if (await _sprintListRepository.DeleteSprintList(sprintList) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when deleting {sprint.SprintName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

