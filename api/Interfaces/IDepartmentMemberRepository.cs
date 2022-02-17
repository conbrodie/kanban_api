using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;

namespace api.Repositories
{
    public interface IDepartmentMemberRepository
    {
        Task<ICollection<DepartmentMember>> GetAllDepartmentMembers(int DepartmentId);
        Task<DepartmentMember> GetDepartmentMember(int DepartmentId);
        Task<bool> RemoveDepartmentMembers(List<int> departmentMemberId);
        Task<bool> RemoveDepartmentMember(int departmentMemberId);
        Task<bool> AddMember(Department department, User user);
        Task<bool> AddMembers(Department department, List<int> userId);
    }
}