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

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {    
        private readonly IUserRepository _userRepository;
         private readonly UserManager<User> _userManager;

        public UsersController(
            IUserRepository userRepository,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IList<User>>> GetUsersForClaims (Claim claim)
        {
            try
            {
                var result = await _userManager.GetUsersForClaimAsync(claim);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving users from the database");
            }

        }

        [HttpGet("{search}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ICollection<UserDTO>>> Search([FromQuery] string searchQuery, [FromQuery] List<int> id)
        {
            try
            {
                var result = await _userRepository.Search(searchQuery, id);

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

