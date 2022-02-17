using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Repositories;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ClaimsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IClaimsRepository _claimsRepository;
        public ClaimsController(UserManager<User> userManager, IUserRepository userRepository, IClaimsRepository claimsRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _claimsRepository = claimsRepository;
        }

        [HttpGet("users/{userId}/claims")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IList<Claim>> GetUserClaims(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            return await _userManager.GetClaimsAsync(user);
        }

        [HttpPost("users/{userId}/claims")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IdentityResult>> CreateClaim(int userId, [FromQuery] string claimType, [FromQuery] string claimValue)
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Id != userId) 
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _claimsRepository.CreateClaim(user, claimType, claimValue);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Something went wrong when creating {claimType + "," + claimValue} for {user.Email}");
                return StatusCode(500, ModelState);
            }

            return Ok(result);

        }

        [HttpPost("claims")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IdentityResult>> CreateClaimForUsers([FromQuery] string type, [FromQuery] string value, [FromQuery] List<int> userId)
        {

            var users = await _userRepository.GetUsersAsync(userId);

            if (users == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _claimsRepository.CreateClaimForUsers(type, value, users);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Something went wrong when creating claim: {type + "," + value} for users");
                return StatusCode(500, ModelState);
            }

            return Ok(result);

        }
    }
}

