using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;

namespace api.Repositories
{
    public interface ISprintListRepository
    {
        Task<ICollection<SprintList>> GetListsForSprint(int SprintId);
        Task<SprintList> GetSprintList(int SprintListId);
        Task<bool> CreateSprintList(SprintList sprintList);
        Task<bool> UpdateSprintList(int SprintListId, SprintList sprintList);
        Task<bool> DeleteSprintList(SprintList sprintList);
        Task<bool> Save();
    }
}