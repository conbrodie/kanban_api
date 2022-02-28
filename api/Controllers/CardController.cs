using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using api.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers 
{
    [Route("api/[controller]")]
    [ValidateModel]
    public class CardController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userManager;
        private readonly ISprintListRepository _sprintListRepository;
        private readonly IMapper _mapper;

        public CardController(ICardRepository cardRepository, IUserRepository userManager, ISprintListRepository sprintListRepository, IMapper mapper) 
        {
            _cardRepository = cardRepository;
            _userManager = userManager;
            _sprintListRepository = sprintListRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all cards for a given sprint list.
        /// </summary>
        /// <returns>A list of cards.</returns>
        /// <response code="200">List of all the cards.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ValidateSprintListExists]
        [HttpGet]
        public async Task<ActionResult<ICollection<CardDTO>>> GetCards([Required] int sprintListId) 
        {
            var cards = await _cardRepository.GetCards(sprintListId);

            ICollection<CardDTO> cardDTOs = _mapper.Map<ICollection<Card>, ICollection<CardDTO>>(cards);

            return Ok(cardDTOs);
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
        [ValidateCardExists]
        [HttpGet("{cardId}")]
        public async Task<ActionResult<CardDTO>> GetCard([Required] int cardId) 
        {
            var card = await _cardRepository.GetCard(cardId);

            var cardDTO = _mapper.Map<CardDTO>(card);

            return Ok(cardDTO);
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
        [ValidateCardExists]
        [HttpGet("{cardId}/members/all")]
        public async Task<ActionResult<ICollection<CardMemberDTO>>> GetCardMembers([Required] int cardId) 
        {
            var members = await _cardRepository.GetCardMembers(cardId);

            ICollection<CardMemberDTO> memberDTOs = _mapper.Map<ICollection<CardMember>, ICollection<CardMemberDTO>>(members);

            return Ok(memberDTOs);
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
        [ValidateCardExists]
        [ValidateCardMemberExists]
        [HttpGet("{cardId}/members/{userId}")]
        public async Task<ActionResult<ICollection<CardMemberDTO>>> GetCardMember([Required] int userId) 
        {
            var member = await _cardRepository.GetCardMember(userId);

            var cardMemberDTO = _mapper.Map<CardMemberDTO>(member);

            return Ok(cardMemberDTO);
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
        [ValidateCardExists]
        [HttpPost("{cardId}/members")]
        public async Task<ActionResult> AddCardMember([Required] int cardId, [Required, FromBody] CardMember cardMember) 
        {
            var isMember = await _cardRepository.IsCardMember(cardMember.UserId);
            var card = await _cardRepository.GetCard(cardId);
            var user = await _userManager.GetUserByIdAsync(cardMember.UserId);

            if (isMember)
            {
                ModelState.AddModelError(string.Empty, "The user is already a member of this card.");
                return new BadRequestError(ModelState);
            }

            if (user == null) 
            {
                ModelState.AddModelError(string.Empty, "The card member was not found.");
                return new NotFoundError(ModelState);
            }

            if (await _cardRepository.AddMember(card, user) == false)
            {
               return new InternalServerError();
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
        [ValidateCardExists]
        [ValidateCardMemberExists]
        [HttpDelete("{cardId}/members")]
        public async Task<ActionResult> RemoveCardMember([Required] int cardId, [Required, FromQuery] int userId)
        {
            var member = await _cardRepository.GetCardMember(userId);

            if (await _cardRepository.RemoveMember(member) == false)
            {
                return new InternalServerError();
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
        [ValidateUserExists]
        [HttpPost]
        public async Task<ActionResult> CreateCard([Required, FromBody] Card card, [Required, FromQuery] int userId) 
        {
            var user = _userManager.GetUserByIdAsync(userId).Result;
            var sprintList = _sprintListRepository.GetSprintList(card.SprintListId).Result;

            if (sprintList == null) 
            {
                ModelState.AddModelError(string.Empty, "The Sprint list was not found.");
                return new NotFoundError(ModelState);
            }

            if (await _cardRepository.CreateCard(card, user) == false)
            {
                return new InternalServerError();
            }

            return CreatedAtAction(nameof(GetCard), new { CardId = card.CardId }, card);
        }

         /// <summary>
        /// Updates a card.
        /// </summary>
        /// <returns>An updated card.</returns>
        /// <response code="204">Updated the card.</response>
        /// <response code="400">One or more validation errors occurred.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Something when wrong when updating the card.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ValidateCardExists]
        [HttpPut("{cardId}")]
        public async Task<ActionResult> UpdateCard([Required] int cardId, [Required, FromBody] Card card) 
        {
            if (cardId != card.CardId)
            {
                ModelState.AddModelError(string.Empty, "CardId in request body does not match path ID.");
                return new BadRequestError(ModelState);
            }

            if (await _cardRepository.UpdateCard(card) == false)
            {
                return new InternalServerError();
            }

            return NoContent();
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
        [ValidateCardExists]
        [HttpDelete("{cardId}")]
        public async Task<ActionResult> DeleteCard([Required] int cardId) 
        {
            var card = await _cardRepository.GetCard(cardId);

            if (await _cardRepository.DeleteCard(card) == false)
            {
                return new InternalServerError();
            }

            return NoContent();
        }
    }
}