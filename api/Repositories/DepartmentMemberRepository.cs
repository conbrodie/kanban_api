using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class DepartmentMemberRepository : IDepartmentMemberRepository
    {
        private readonly AppDbContext _context;
        public DepartmentMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddMember(Department department, User user)
        {
            var departmentMember = new DepartmentMember()
            {
                User = user,
                Department = department
            };
            await _context.DepartmentMembers.AddAsync(departmentMember);

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
                await _context.AddAsync(departmentMember);
            }

            return await Save();
        }

        public async Task<ICollection<DepartmentMember>> GetAllDepartmentMembers(int departmentId)
        {
            return await _context.DepartmentMembers.Where(dep => dep.DepartmentId == departmentId).ToListAsync();
        }

        public async Task<DepartmentMember> GetDepartmentMember(int departmentMemberId)
        {
            return await _context.DepartmentMembers.FindAsync(departmentMemberId);
        }

        public async Task<bool> RemoveDepartmentMembers(List<int> departmentMemberId)
        {
            var membersToRemove = await _context.DepartmentMembers.Where(u => departmentMemberId.Contains(u.DepartmentMemberId)).ToListAsync();
            _context.RemoveRange(membersToRemove);
            return await Save();
        }

        public async Task<bool> RemoveDepartmentMember(int departmentMemberId)
        {
            var memberToRemove = await _context.DepartmentMembers.FindAsync(departmentMemberId);
           _context.Remove(memberToRemove);
           return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            return saved >= 0 ? true : false;
        }
    }
}