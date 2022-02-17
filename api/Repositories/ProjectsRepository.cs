using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace api.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ProjectsRepository(AppDbContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<bool> DeleteProject(Project project)
        {
            _context.Remove(project);
            return await Save();
        }

        /// <summary>
        /// Gets all projects by approved status
        /// </summary>
        /// <param name="isApproved"></param>
        /// <returns>All projects</returns>

        public async Task<ICollection<Project>> GetProjects(bool isApproved)
        {
            var projects = await _context.Projects.Where(p => p.Approved == isApproved).ToListAsync();

            foreach (var project in projects)
            {
                await _context.Entry(project)
                            .Collection(proj => proj.Members)
                            .Query()
                            .Include(ProjectMember => ProjectMember.User)
                            .AsSplitQuery()
                            .LoadAsync();
            }

            return projects;
        }

        /// <summary>
        /// Gets a project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>Project</returns>

        public async Task<Project> GetProject(int ProjectId)
        {
            var Project = await _context.Projects
                .Include(project => project.Members)
                    .ThenInclude(user => user.User)
                .Include(project => project.Departments)
                    .ThenInclude(department => department.Department)
                .Include(project => project.Sprints)
                    .ThenInclude(sprint => sprint.SprintLists)
                    .ThenInclude(list => list.Cards.OrderBy(card => card.Order))
                .AsSplitQuery()
                .Where(p => p.ProjectId == ProjectId)
                .FirstOrDefaultAsync();

            return Project;
        }

        /// <summary>
        /// Gets all project members
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>Project members</returns>

        public async Task<ICollection<Project>> GetProjectMembers(int projectId)
        {
            return await _context.Projects
                .Include(pm => pm.Members)
                    .ThenInclude(u => u.User)
                .Where(p => p.ProjectId == projectId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all projects
        /// </summary>
        /// <returns>All projects mapped using ProjectPreviewDTO</returns>

        public async Task<ICollection<ProjectPreviewDTO>> GetProjectPreviews()
        {
            var previews = await _context.Projects
                .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                .Include(x => x.Departments)
                    .ThenInclude(x => x.Department)
                .ToListAsync();

            var projectPreviewDTO = _mapper.Map<List<ProjectPreviewDTO>>(previews);

            return projectPreviewDTO;
        } 

        public async Task<bool> CreateProject(Project project, List<int> userId, List<int> departmentId)
        {
            var users = await _context.Users.Where(user => userId.Contains(user.Id)).ToListAsync();
            var departments = await _context.Departments.Where(dep => departmentId.Contains(dep.DepartmentId)).ToListAsync();

            if (!users.Any(user => userId.Contains(user.Id))) {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "User's not found. Ensure you are supplying valid userId's.");
            }

            if (departmentId != null && !departments.Any(dep => departmentId.Contains(dep.DepartmentId))) { 
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Department's not found. Ensure you are supplying valid departmentId's.");
            }

            foreach (var user in users)
            {
                // user(s) who have created the project
                var projectMember = new ProjectMember()
                {
                    User = user,
                    Project = project
                };
                await _context.AddAsync(projectMember);
            }

            foreach (var department in departments)
            {
                var projectDepartment = new ProjectDepartment()
                {
                    Department = department,
                    Project = project
                };
                await _context.AddAsync(projectDepartment);
            }

            await _context.AddAsync(project);

            return await Save();
        }

        /// <summary>
        /// Updates a project.
        /// </summary>
        /// <param name="project">Project object being updated</param>
        /// <param name="projectCreatorId">A List of user id's (users who created the project)</param>
        /// <param name="projectSponsorId">A list of user id's (users who sponsored the project)</param>
        /// <param name="projectDepartmentId">A list of department id's (departments the project is in)</param>
        /// <returns>Updated record in the database</returns>

        public async Task<bool> UpdateProject(Project project, List<int> userId, List<int> departmentId)
        {
            var users = await _context.Users.Where(user => userId.Contains(user.Id)).ToListAsync();
            var departments = await _context.Departments.Where(dep => departmentId.Contains(dep.DepartmentId)).ToListAsync();

            if (!users.Any(user => userId.Contains(user.Id))) {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "User's not found. Ensure you are supplying valid userId's.");
            }

            if (departmentId != null && !departments.Any(dep => departmentId.Contains(dep.DepartmentId))) { 
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Department's not found. Ensure you are supplying valid departmentId's.");
            }

            var projectMembersToDelete = await _context.ProjectMembers.Where(p => p.ProjectId == project.ProjectId).ToListAsync();
            var projectDepartmentsToDelete = await _context.ProjectDepartments.Where(p => p.ProjectId == project.ProjectId).ToListAsync();

            _context.RemoveRange(projectMembersToDelete);
            _context.RemoveRange(projectDepartmentsToDelete);

            foreach (var user in users)
            {
                var projectMember = new ProjectMember()
                {
                    User = user,
                    Project = project
                };
                await _context.AddAsync(projectMember);
            }

            foreach (var department in departments)
            {
                var projectDepartment = new ProjectDepartment()
                {
                    Department = department,
                    Project = project
                };
                await _context.AddAsync(projectDepartment);
            }
            _context.Update(project);

            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved >= 0 ? true : false;
        }

        public async Task<bool> PatchProject(Project project, JsonPatchDocument<Project> patchDoc)
        {
            patchDoc.ApplyTo(project);
            return await Save();
        }
    }
}
