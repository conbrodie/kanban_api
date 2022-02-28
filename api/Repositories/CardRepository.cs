using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories 
{
    public class CardRepository : ICardRepository
    {
        AppDbContext _context;

        IMapper _mapper;

        public CardRepository( AppDbContext context,  IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateCard(Card card, User user)
        {
           var cardMember = new CardMember()
            {
                User = user,
                Card = card
            };

            _context.Add(cardMember);

            _context.Add(card);

            return await Save();
        }

        public async Task<bool> DeleteCard(Card card)
        {
            _context.Remove(card);
            return await Save();
        }

        public async Task<Card> GetCard(int cardId)
        {
            return await _context.Cards.Where(card => card.CardId == cardId)
                .Include(m => m.Members)
                    .ThenInclude(u => u.User)
                .Include(ptc => ptc.Comments)
                    .ThenInclude(u => u.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<CardMember>> GetCardMembers(int cardId)
        {
           return await _context.CardMembers.Include(cm => cm.User).Where(c => c.CardId == cardId).ToListAsync();
        }

        public async Task<CardMember> GetCardMember(int userId)
        {
            return await _context.CardMembers.Include(c => c.User).Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Card>> GetCards(int sprintListId)
        {
            return await _context.Cards.Where(c => c.SprintListId == sprintListId).ToListAsync();
        }

        public async Task<bool> UpdateCard(Card card)
        {
            _context.Update(card);
            return await Save();
        }
      

        public async Task<bool> AddMember(Card card, User user)
        {
           var newMember = new CardMember 
           {
               Card = card,
               User = user
           };

           _context.Add(newMember);
           return await Save();
        }

        public async Task<bool> RemoveMember(CardMember cardMember)
        {
            _context.Remove(cardMember);
            return await Save();
        }

        public async Task<bool> IsCardMember(int userId)
        {
           var member = await _context.CardMembers.Where(cm => cm.UserId == userId).FirstOrDefaultAsync();
           return member == null ? false : true;
        }

          public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved >= 0 ? true : false;
        }

    }
}