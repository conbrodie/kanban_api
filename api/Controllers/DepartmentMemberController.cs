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

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class DepartmentMembersController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentMemberRepository _departmentMemberRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public DepartmentMembersController(IDepartmentRepository departmentRepository, IDepartmentMemberRepository departmentMemberRepository, UserManager<User> userManager, IUserRepository userRepository)
        {
            _departmentRepository = departmentRepository;
            _departmentMemberRepository = departmentMemberRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [ProducesResponseType(500)]
        [AllowAnonymous]
        [HttpGet("departments/{departmentId}/members")]
        public async Task<ICollection<DepartmentMember>> GetAllDepartmentMemberMembers(int departmentId)
        {
            return await _departmentMemberRepository.GetAllDepartmentMembers(departmentId);
        }


        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("departments/{departmentId}/members/{departmentMemberId}")]
        public async Task<ActionResult<DepartmentMember>> GetDepartmentMember(int departmentId, [FromBody] int departmentMemberId)
        {
            var department = await _departmentMemberRepository.GetDepartmentMember(departmentMemberId);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        [HttpPost("departments/{DepartmentId}/members")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> AddMember(int DepartmentId, [FromBody] DepartmentMember departmentMember)
        {

             var user = await _userRepository.GetUserByIdAsync(departmentMember.UserId);
             var department = await _departmentRepository.GetDepartment(DepartmentId);

            if (departmentMember == null)
            {
                return BadRequest(ModelState);
            }

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (department == null)
            {
                return NotFound("Department not found");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _departmentMemberRepository.AddMember(department, user) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when updating {department.DepartmentName}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetDepartmentMember), new { DepartmentMemberId = departmentMember.DepartmentId }, departmentMember);
        }

    }
}

