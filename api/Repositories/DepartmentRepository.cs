using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public DepartmentRepository(AppDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Department> GetDepartment(int DepartmentId)
        {
            return await _context.Departments
                .Include(d => d.Members)
                    .ThenInclude(dm => dm.User)
                .Where(d => d.DepartmentId == DepartmentId)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<Department>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<List<string>> GetDepartmentNames()
        {
            return await _context.Departments.Select(x => x.DepartmentName).ToListAsync();
        }
       
        public async Task<ICollection<Department>> Search(string searchQuery, List<int> departmentId)
        {
            IQueryable<Department> query = _context.Departments;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(d => d.DepartmentName.Contains(searchQuery));          
            }

            // filter out users that have already been selected
            var filteredDepartments = query.Where(d => !departmentId.Contains(d.DepartmentId)) 
                .Take(10) 
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            return await filteredDepartments;
        }

        public async Task<ICollection<DepartmentMember>> GetAllDepartmentMembers(int departmentId)
        {
            return await _context.DepartmentMembers
                .Where(dep => dep.DepartmentId == departmentId)
                .Include(u => u.User)
                .ToListAsync();
        }

        public async Task<DepartmentMember> GetDepartmentMember(int departmentMemberId)
        {
            return await _context.DepartmentMembers
                .Where(dm => dm.DepartmentMemberId == departmentMemberId)
                .Include(u => u.User)
                .FirstOrDefaultAsync();
        }


        public async Task<bool> AddMember(Department department, User user)
        {
            var departmentMember = new DepartmentMember()
            {
                User = user,
                Department = department
            };
            _context.DepartmentMembers.Add(departmentMember);

            return await Save();
        }

        public async Task<bool> AddMembers(Department department, List<int> userId)
        {
            var membersToBeAdded = await _context.Users.Where(u => userId.Contains(u.Id)).ToListAsync();

            foreach (var user in membersToBeAdded) 
            {
                var departmentMember = new DepartmentMember()
                {
                    User = user,
                    Department = department
                };
                _context.Add(departmentMember);
            }

            return await Save();
        }

        public async Task<bool> RemoveDepartmentMembers(List<int> departmentMemberId)
        {
            var membersToRemove = await _context.DepartmentMembers.Where(u => departmentMemberId.Contains(u.DepartmentMemberId)).ToListAsync();
            _context.RemoveRange(membersToRemove);
            return await Save();
        }

        public async Task<bool> RemoveDepartmentMember(int DepartmentMemberId)
        {
            var memberToRemove = await _context.DepartmentMembers.FindAsync(DepartmentMemberId);
           _context.Remove(memberToRemove);
           return await Save();
        }


        public async Task<bool> CreateDepartment(Department department, User user)
        {
            // Create new department with a new department member as the admin of that department
            var admin = new DepartmentMember()
            {
                User = user,
                Department = department
            };

            _context.Add(admin);

            _context.Add(department);

            return await Save();  
        }

          public async Task<bool> PatchDepartment(Department department, JsonPatchDocument<Department> patchDoc)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> UpdateDepartment(Department department, List<int> userId)
        {
            var membersToAdd = await _context.Users.Where(u => userId.Contains(u.Id)).ToListAsync(); 

            var membersToRemove = await _context.DepartmentMembers.Where(d => d.DepartmentId == department.DepartmentId).ToListAsync();

            _context.RemoveRange(membersToRemove);

            foreach(var user in membersToAdd) 
            {
                var departmentMember = new DepartmentMember()
                {
                    User = user,
                    Department = department
                };
                _context.Add(departmentMember);
            }
            _context.Update(department);

            return await Save();      
        }

        public async Task<bool> DeleteDepartment(Department department)
        {
            _context.Remove(department);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved >= 0 ? true : false;
        }

        public async Task<bool> IsDepartmentMember(int userId)
        {
           var member = await _context.DepartmentMembers.Where(dm => dm.UserId == userId).FirstOrDefaultAsync();
           return member == null ? false : true;
        }
    }
}
