using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Repositories;
using api.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ValidateModel]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentMemberRepository _departmentMemberRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private IMapper _mapper;

        public DepartmentsController(IDepartmentRepository departmentRepository, IDepartmentMemberRepository departmentMemberRepository, UserManager<User> userManager, IUserRepository userRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _departmentMemberRepository = departmentMemberRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all departments.
        /// </summary>
        /// <returns>A list of departments.</returns>
        /// <response code="200">List of all the departments with id, name and badge color.</response>
        [ProducesResponseType(200)]
        [HttpGet]
        public async Task<ActionResult<ICollection<DepartmentPreviewDTO>>> GetDepartments()
        {
            var allDepartments = await _departmentRepository.GetDepartments();
            
            ICollection<DepartmentPreviewDTO> departmentPreviewDTOs = _mapper.Map<ICollection<Department>, ICollection<DepartmentPreviewDTO>>(allDepartments);

            return Ok(departmentPreviewDTOs);
        }

        /// <summary>
        /// Gets all department names.
        /// </summary>
        /// <returns>A list of department names.</returns>
        /// <response code="200">List of all the departments names.</response>
        [ProducesResponseType(200)]
        [HttpGet("names")]
        public async Task<ActionResult<List<string>>> GetDepartmentNames()
        {
            return Ok(await _departmentRepository.GetDepartmentNames());
        }

        /// <summary>
        /// Gets a department.
        /// </summary>
        /// <returns>A department.</returns>
        /// <response code="200">Current information for a depatment.</response>
        /// <response code="404">Department not found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ValidateDepartmentExists]
        [HttpGet("{departmentId}")]
        public async Task<ActionResult<DepartmentDTO>> GetDepartment([Required] int departmentId)
        {
            var department = await _departmentRepository.GetDepartment(departmentId);

            var departmentDTO = _mapper.Map<DepartmentDTO>(department);

            return Ok(departmentDTO);
        }

        /// <summary>
        /// Gets all members for a specific department.
        /// </summary>
        /// <returns>A list of members.</returns>
        /// <response code="200">List of all members in a department.</response>
        /// <response code="404">Department not found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ValidateDepartmentExists]
        [HttpGet("{departmentId}/members/all")]
        public async Task<ActionResult<ICollection<DepartmentMemberDTO>>> GetAllDepartmentMembers([Required] int departmentId)
        {
            var departmentMembers = await _departmentRepository.GetAllDepartmentMembers(departmentId);

            ICollection<DepartmentMemberDTO> departmentMemberDTOs = _mapper.Map<ICollection<DepartmentMember>, ICollection<DepartmentMemberDTO>>(departmentMembers);

            return Ok(departmentMemberDTOs);
        }

        /// <summary>
        /// Gets a department member. 
        /// </summary>
        /// <returns>A department member.</returns>
        /// <response code="200">A department member.</response>
        /// <response code="404">Not found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ValidateDepartmentExists]
        [ValidateDepartmentMemberExists]
        [HttpGet("{departmentId}/members/{departmentMemberId}")]
        public async Task<ActionResult<DepartmentMemberDTO>> GetDepartmentMember([Required] int departmentId, [Required] int departmentMemberId)
        {
            var departmentMember = await _departmentRepository.GetDepartmentMember(departmentMemberId);

            var departmentMemberDTO = _mapper.Map<DepartmentMemberDTO>(departmentMember);

            return Ok(departmentMemberDTO);
        }

        /// <summary>
        /// Updates a department.
        /// </summary>
        /// <returns>An updated department.</returns>
        /// <response code="204">Department updated.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Department not found.</response>
        /// <response code="500">Something when wrong when updating the department.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateDepartmentExists]
        [ValidateUsersExists]
        [HttpPut("{departmentId}")]
        public async Task<ActionResult> UpdateDepartment([Required] int departmentId, [FromBody, Required] Department department, [FromQuery] List<int> userId)
        {
            if (departmentId != department.DepartmentId)
            {
                ModelState.AddModelError(string.Empty, "DepartmentId in request body does not match path ID.");
                return new BadRequestError(ModelState);
            }

            if (await _departmentRepository.UpdateDepartment(department, userId) == false)
            {
                return new InternalServerError();
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a department.
        /// </summary>
        /// <returns>A created department.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /departments
        ///     {
        ///        "DepartmentName": "Name",
        ///        "DepartmentDescription": "Description",
        ///        "Color": "#fff",
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Created the department.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not found.</response>
        /// <response code="500">Something went wrong when creating the department.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateUserExists]
        [HttpPost]
        public async Task<ActionResult> CreateDepartment([FromBody, Required] Department department, [FromQuery, Required] int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (!await _departmentRepository.CreateDepartment(department, user))
            {
               return new InternalServerError();
            }

            return CreatedAtAction(nameof(GetDepartment), new { DepartmentId = department.DepartmentId }, department);
        }

        /// <summary>
        /// Adds a member to a department.
        /// </summary>
        /// <returns>A department member.</returns>
        /// <response code="201">Added the member to the department.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when adding the member to the department.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateDepartmentExists]
        [HttpPost("{departmentId}/members")]
        public async Task<ActionResult> AddMember([Required] int departmentId, [FromBody, Required] DepartmentMember departmentMember)
        {
            var isMember = await _departmentRepository.IsDepartmentMember(departmentMember.UserId);
            var user = await _userRepository.GetUserByIdAsync(departmentMember.UserId);
            var department = await _departmentRepository.GetDepartment(departmentId);

            if (isMember) 
            {
                ModelState.AddModelError("", "This user is already a member of this department");
                return new NotFoundError(ModelState);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "The user was not found");
                return new NotFoundError(ModelState);
            }

            if (!await _departmentRepository.AddMember(department, user))
            {
                return new InternalServerError();
            }

            return CreatedAtAction(nameof(GetDepartmentMember), new { DepartmentMemberId = departmentMember.DepartmentId }, departmentMember);
        }

        /// <summary>
        /// Removes a member from a department.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Removed the member.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when removing the member.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateDepartmentExists]
        [ValidateDepartmentMemberExists]
        [HttpDelete("{departmentId}/members")]
        public async Task<ActionResult> RemoveMember([Required] int departmentId, [FromQuery, Required] int departmentMemberId)
        {
            if (await _departmentRepository.RemoveDepartmentMember(departmentMemberId) == false) 
            {
                return new InternalServerError();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a department.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Deleted the department.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something went wrong when deleting the department.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ValidateDepartmentExists]
        [HttpDelete("{departmentId}")]
        public async Task<ActionResult> DeleteDepartment([Required] int departmentId)
        {
            var department = await _departmentRepository.GetDepartment(departmentId);

            if (await _departmentRepository.DeleteDepartment(department) == false)
            {
                return new InternalServerError();
            }

            return NoContent();
        }

        // [ProducesResponseType(200)]
        // [ProducesResponseType(404)]
        // [ProducesResponseType(500)]
        // [HttpGet("search")]
        // public async Task<ActionResult<ICollection<Department>>> Search([FromQuery] string searchQuery, [FromQuery] List<int> departmentId)
        // {
        //     try
        //     {
        //         var result = await _departmentRepository.Search(searchQuery, departmentId);

        //         if (result.Any())
        //         {
        //             return Ok(result);
        //         }

        //         return NotFound();
        //     }
        //     catch (Exception)
        //     {
        //         return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
        //     }
        // }
    }
}

