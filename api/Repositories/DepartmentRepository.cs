using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        public async Task<bool> CreateDepartment(Department department, User user)
        {
            // Create new department with a new department member as the admin of that department
            var admin = new DepartmentMember()
            {
                User = user,
                Department = department
            };

            await _context.AddAsync(admin);

            await _context.AddAsync(department);

            return await Save();  
        }

        public async Task<bool> DeleteDepartment(Department department)
        {
            _context.Remove(department);
            return await Save();
        }

        public async Task<Department> GetDepartment(int DepartmentId)
        {
            return await _context.Departments.FindAsync(DepartmentId);
        }

        public async Task<ICollection<DepartmentDTO>> GetDepartments()
        {
            var departments =  await _context.Departments.ToListAsync();
            
            return _mapper.Map<List<DepartmentDTO>>(departments);
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
                await _context.AddAsync(departmentMember);
            }
            await _context.AddAsync(department);

            return await Save();      
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved >= 0 ? true : false;
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

            foreach (var department in filteredDepartments.Result)
            {
                _mapper.Map<DepartmentDTO>(department);
            }

            return await filteredDepartments;
        }

        public async Task<List<string>> GetDepartmentNames()
        {
            return await _context.Departments.Select(x => x.DepartmentName).ToListAsync();
        }
    }
}
