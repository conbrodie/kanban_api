using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class SprintListRepository : ISprintListRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SprintListRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateSprintList(SprintList sprintList)
        {
            await _context.AddAsync(sprintList);
            return await Save();
        }

        public async Task<bool> DeleteSprintList(SprintList sprintList)
        {
            _context.Remove(sprintList);
            return await Save();
        }

        public async Task<ICollection<SprintList>> GetListsForSprint(int SprintId)
        {
            return await _context.SprintLists.Where(sprintList => sprintList.SprintId == SprintId).ToListAsync();
        }

        public async Task<SprintList> GetSprintList(int SprintListId)
        {
            return await _context.SprintLists
                .Include(c => c.Cards)
                    .ThenInclude(c => c.Comments)
                        .ThenInclude(m => m.User)
                 .Include(c => c.Cards)
                    .ThenInclude(c => c.Members)
                        .ThenInclude(m => m.User)
                .Where(sprintList => sprintList.SprintListId == SprintListId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved >= 0 ? true : false;
        }

        public async Task<bool> UpdateSprintList(int SprintListId, SprintList sprintList)
        {
            throw new System.NotImplementedException();
        }
    }
}