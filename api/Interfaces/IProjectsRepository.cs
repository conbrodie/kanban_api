using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.JsonPatch;
using api.DTO;

namespace api.Repositories
{
    public interface IProjectsRepository
    {
        Task<ICollection<Project>> GetProjects(bool status);
        Task<ICollection<ProjectPreviewDTO>> GetProjectPreviews();
        Task<Project> GetProject(int projectId);
        Task<ICollection<Project>> GetProjectMembers(int projectId);
        Task<bool> PatchProject(Project project, JsonPatchDocument<Project> patchDoc);
        Task<bool> CreateProject(Project project, List<int> userId, List<int> departmentId);
        Task<bool> UpdateProject(Project project, List<int> userId, List<int> departmentId);
        Task<bool> DeleteProject(Project project);
    }
}