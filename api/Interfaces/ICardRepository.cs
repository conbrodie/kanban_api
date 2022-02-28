using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTO;
using api.Models;

namespace api.Repositories
{
    public interface ICardRepository
    {
        Task<ICollection<Card>> GetCards(int sprintListId);
        Task<Card> GetCard(int cardId);
        Task<ICollection<CardMember>> GetCardMembers(int cardId);
        Task<CardMember> GetCardMember(int userId);
        Task<bool> IsCardMember(int userId);
        Task<bool> AddMember(Card card, User user);
        Task<bool> RemoveMember(CardMember cardMember);
        Task<bool> CreateCard(Card card, User user);
        Task<bool> UpdateCard(Card card);
        Task<bool> DeleteCard(Card card);
    }
}