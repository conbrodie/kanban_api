using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userManager;
         private readonly ISprintListRepository _sprintListRepository;

        public CardController(ICardRepository cardRepository, IUserRepository userManager, ISprintListRepository sprintListRepository) 
        {
            _cardRepository = cardRepository;
            _userManager = userManager;
            _sprintListRepository = sprintListRepository;
        }

        /// <summary>
        /// Gets all cards for a given sprint list.
        /// </summary>
        /// <returns>A list of cards.</returns>
        /// <response code="200">List of all the cards.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<Card>> GetCards([Required] int sprintListId) 
        {
            var sprintList = await _sprintListRepository.GetSprintList(sprintListId);

            if (sprintList == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint list not found.");
            }

            var cards = await _cardRepository.GetCards(sprintListId);

            return Ok(cards);
        }

        /// <summary>
        /// Gets a card.
        /// </summary>
        /// <returns>A card.</returns>
        /// <response code="200">Returns a card.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{cardId}")]
        public async Task<ActionResult<Card>> GetCard([Required] int cardId) 
        {
            var card = await _cardRepository.GetCard(cardId);

            if (card == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Card not found.");
            }

            return Ok(card);
        }

        /// <summary>
        /// Gets all card members.
        /// </summary>
        /// <returns>A list of card members.</returns>
        /// <response code="200">Returns a list of card members.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{cardId}/members/all")]
        public async Task<ActionResult<ICollection<UserDTO>>> GetCardMembers([Required] int cardId) 
        {
            var card = await _cardRepository.GetCard(cardId);

            if (card == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Card not found.");
            }

            var members = await _cardRepository.GetCardMembers(cardId);

            return Ok(members);
        }

        /// <summary>
        /// Gets a card member.
        /// </summary>
        /// <returns>A card member.</returns>
        /// <response code="200">Returns a card member.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{cardId}/members/{userId}")]
        public async Task<ActionResult<ICollection<UserDTO>>> GetCardMember([Required] int userId) 
        {
            var member = await _cardRepository.GetCardMember(userId);

            if (member == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Member not found.");
            }

            return Ok(member);
        }

         /// <summary>
        /// Adds a member to the card.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">Added the member to the card.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something when wrong when adding the member.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpPost("{cardId}/members")]
        public async Task<ActionResult> AddCardMember([Required] int cardId, [Required, FromBody] CardMember cardMember) 
        {
            var card = await _cardRepository.GetCard(cardId);

            if (card == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Card not found.");
            }

            var user = await _userManager.GetUserByIdAsync(cardMember.UserId);

            if (user == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "User not found.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _cardRepository.AddMember(card, user) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when adding member {user.FirstName + " " + user.LastName}");
                return StatusCode(500, ModelState);
            }

           return CreatedAtAction(nameof(GetCardMember), new { CardMemberId = cardMember.CardId }, cardMember);
        }

         /// <summary>
        /// Removes a member from a card.
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
        [HttpDelete("{cardId}/members")]
        public async Task<ActionResult> RemoveCardMember([Required] int cardId, [Required, FromQuery] int userId)
        {
            var card = await _cardRepository.GetCard(cardId);

            if (card == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Card not found.");
            }

            var member = await _cardRepository.GetCardMember(userId);

            if (member == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "User not found.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _cardRepository.RemoveMember(member) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when removing member {member.User.FirstName + " " + member.User.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a card.
        /// </summary>
        /// <returns>A new card.</returns>
        /// <response code="201">Created the card.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something when wrong when creating the card.</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpPost]
        public async Task<ActionResult> CreateCard([Required, FromBody] Card card, [Required, FromQuery] int userId) 
        {
            if (card == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userManager.GetUserByIdAsync(userId).Result;

            if (user == null) 
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "User not found.");
            }

            var sprintList = _sprintListRepository.GetSprintList(card.SprintListId).Result;

            if (sprintList == null) 
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Sprint list not found.");
            }

            if (await _cardRepository.CreateCard(card, user) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when creating {card.CardTitle}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetCard), new { CardId = card.CardId }, card);
        }

        /// <summary>
        /// Removes a card.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Removed the card.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something when wrong when deleting the card.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpDelete("{cardId}")]
        public async Task<ActionResult> DeleteCard([Required] int cardId) 
        {
            var card = await _cardRepository.GetCard(cardId);

            if (card == null) 
            {
               throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Card not found.");
            }

            if (await _cardRepository.DeleteCard(card) == false)
            {
                ModelState.AddModelError("", $"Something went wrong when deleting {card.CardTitle}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}