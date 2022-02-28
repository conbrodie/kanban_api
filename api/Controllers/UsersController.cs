using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System;
using System.Linq;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Http;
using api.DTO;
using AutoMapper;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {    
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private IMapper _mapper;

        public UsersController(
            IUserRepository userRepository,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a list of all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        /// <response code="200">Returns a list of users.</response>
        [ProducesResponseType(200)]
        [HttpGet("all")]
        public async Task<ActionResult<ICollection<User>>> GetUsers ()
        {
           return Ok(await _userRepository.GetUsers());

        }

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <returns>A user.</returns>
        /// <response code="200">Returns a user.</response>
        /// <response code="400">One or more validation errors have occured.</response>
        /// <response code="404">Not found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser (int userId)
        {
           var user = await _userRepository.GetUserByIdAsync(userId);

           if (user == null) 
           {
               return NotFound();
           }

           return Ok(user);
        }

        /// <summary>
        /// Registers a user.
        /// </summary>
        /// <returns>A registerd user</returns>
        /// <response code="201">Returns a user.</response>
        /// <response code="400">One or more validation errors have occured.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationModel user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newUser = _mapper.Map<User>(user);

            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                var errors = result.Errors;
                foreach(var err in errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetUser), new { Id = newUser.Id }, newUser);
        }
    }
}

