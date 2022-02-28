using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using api.Models;
using api.Repositories;
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
        public async Task<ActionResult<ICollection<UserDTO>>> GetUsers ()
        {
            var users = await _userRepository.GetUsers();

            ICollection<UserDTO> userDTOs = _mapper.Map<ICollection<User>, ICollection<UserDTO>>(users);

            return Ok(userDTOs);
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
        public async Task<ActionResult<UserDTO>> GetUser (int userId)
        {
           var user = await _userRepository.GetUserByIdAsync(userId);

           if (user == null) 
           {
               return NotFound();
           }

           var userDTO = _mapper.Map<UserDTO>(user);

           return Ok(userDTO);
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

