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
            var previews = await _context.Projects
                .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                .Include(x => x.Departments)
                    .ThenInclude(x => x.Department)
                .Where(p => p.Approved == isApproved)
                .ToListAsync();

            return previews;
        }

        /// <summary>
        /// Gets a project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>Project</returns>

        public async Task<Project> GetProject(int ProjectId)
        {
            var project = await _context.Projects
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

            return project;
        }

        /// <summary>
        /// Gets all project members
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>Project members</returns>

        public async Task<ICollection<ProjectMember>> GetProjectMembers(int projectId)
        {
            return await _context.ProjectMembers
                .Include(u => u.User)
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all projects
        /// </summary>
        /// <returns>All projects mapped using ProjectPreviewDTO</returns>

        public async Task<ICollection<Project>> GetProjectPreviews()
        {
            return await _context.Projects
                .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                .Include(x => x.Departments)
                    .ThenInclude(x => x.Department)
                .ToListAsync();
        } 

        public async Task<bool> CreateProject(Project project, List<int> userId, List<int> departmentId)
        {
            var users = await _context.Users.Where(user => userId.Contains(user.Id)).ToListAsync();
            var departments = await _context.Departments.Where(dep => departmentId.Contains(dep.DepartmentId)).ToListAsync();

            foreach (var user in users)
            {
                // user(s) who have created the project
                var projectMember = new ProjectMember()
                {
                    User = user,
                    Project = project
                };
                _context.Add(projectMember);
            }

            foreach (var department in departments)
            {
                var projectDepartment = new ProjectDepartment()
                {
                    Department = department,
                    Project = project
                };
                _context.Add(projectDepartment);
            }

            _context.Add(project);

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
                _context.Add(projectMember);
            }

            foreach (var department in departments)
            {
                var projectDepartment = new ProjectDepartment()
                {
                    Department = department,
                    Project = project
                };
                _context.Add(projectDepartment);
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
