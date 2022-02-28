using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace api.Repositories
{
    public interface IDepartmentRepository
    {
        Task<ICollection<Department>> GetDepartments();
        Task<Department> GetDepartment(int DepartmentId);
        Task<List<string>> GetDepartmentNames();
        Task<ICollection<Department>> Search(string searchQuery, List<int> departmentId);
        Task<ICollection<DepartmentMember>> GetAllDepartmentMembers(int DepartmentId);
        Task<DepartmentMember> GetDepartmentMember(int DepartmentId);
        Task<bool> IsDepartmentMember(int UserId);
        Task<bool> AddMember(Department department, User user);
        Task<bool> AddMembers(Department department, List<int> userId); 
        Task<bool> RemoveDepartmentMembers(List<int> departmentMemberId);
        Task<bool> RemoveDepartmentMember(int departmentMemberId);
        Task<bool> CreateDepartment(Department department, User user);
        Task<bool> PatchDepartment(Department department, JsonPatchDocument<Department> patchDoc);
        Task<bool> UpdateDepartment(Department department, List<int> userId);
        Task<bool> DeleteDepartment(Department department);
    }
}