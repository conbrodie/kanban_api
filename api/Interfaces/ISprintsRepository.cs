using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;

namespace api.Repositories
{
    public interface ISprintsRepository
    {
        Task<ICollection<Sprint>> GetSprintsForProject(int ProjectId);
        Task<Sprint> GetSprint(int SprintId);
        Task<bool> CreateSprint(Sprint sprint);
        Task<bool> UpdateSprint(int SprintId, Sprint sprint);
        Task<bool> DeleteSprint(Sprint sprint);
        Task<bool> Save();
    }
}