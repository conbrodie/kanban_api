using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class SprintRepository : ISprintsRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SprintRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateSprint(Sprint sprint)
        {
           await _context.AddAsync(sprint);
           return await Save();
        }

        public async Task<bool> DeleteSprint(Sprint sprint)
        {
            _context.Remove(sprint);
            return await Save();
        }

        public async Task<Sprint> GetSprint(int SprintId)
        {
            return await _context.Sprints.Where(sprint => sprint.SprintId == SprintId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Sprint>> GetSprintsForProject(int ProjectId)
        {
            return await _context.Sprints.Where(sprint => sprint.ProjectId == ProjectId).ToListAsync();
        }

        public async Task<bool> UpdateSprint(int SprintId, Sprint sprint)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved >= 0 ? true : false;
        }
    }
}