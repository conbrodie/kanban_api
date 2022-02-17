using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Repositories;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentMemberRepository _departmentMemberRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public DepartmentsController(IDepartmentRepository departmentRepository, IDepartmentMemberRepository departmentMemberRepository, UserManager<User> userManager, IUserRepository userRepository)
        {
            _departmentRepository = departmentRepository;
            _departmentMemberRepository = departmentMemberRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [ProducesResponseType(500)]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ICollection<DepartmentDTO>> GetDepartments()
        {
            return await _departmentRepository.GetDepartments();
        }

        [ProducesResponseType(500)]
        [AllowAnonymous]
        [HttpGet]
         [HttpGet("names")]
        public async Task<List<string>> GetDepartmentNames()
        {
            return await _departmentRepository.GetDepartmentNames();
        }

        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("{departmentId}")]
        public async Task<ActionResult<Department>> GetDepartment([FromBody] int departmentId)
        {
            var department = await _departmentRepository.GetDepartment(departmentId);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        [ProducesResponseType(500)]
        [HttpGet("departments/{departmentId}/members/all")]
        public async Task<ICollection<DepartmentMember>> GetAllDepartmentMembers([FromBody] int departmentId)
        {
            return await _departmentMemberRepository.GetAllDepartmentMembers(departmentId);
        }

        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("{departmentId}/members/{departmentMemberId}")]
        public async Task<ActionResult<DepartmentMember>> GetDepartmentMember([FromBody] int departmentMemberId)
        {
            var departmentMember = await _departmentMemberRepository.GetDepartmentMember(departmentMemberId);

            if (departmentMember == null)
            {
                return NotFound();
            }

            return departmentMember;
        }

        [HttpPut("{departmentId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateDepartment(int departmentId, [FromBody] Department department, [FromQuery] List<int> userId)
        {

            if (department == null)
            {
                return BadRequest(ModelState);
            }

            if (departmentId != department.DepartmentId)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _departmentRepository.UpdateDepartment(department, userId) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when updating {department.DepartmentName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateDepartment([FromBody] Department department, [FromQuery] int userId)
        {
            if (department == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (!await _departmentRepository.CreateDepartment(department, user))
            {
                ModelState.AddModelError("", $"Something went wrong when saving {department.DepartmentName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetDepartment), new { DepartmentId = department.DepartmentId }, department);
        }

        [HttpDelete("{departmentId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteDepartment(int departmentId, [FromBody] Department department)
        {
            if (department == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _departmentRepository.DeleteDepartment(department) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when delete {department.DepartmentName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpGet("search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ICollection<Department>>> Search([FromQuery] string searchQuery, [FromQuery] List<int> departmentId)
        {
            try
            {
                var result = await _departmentRepository.Search(searchQuery, departmentId);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
    }
}

