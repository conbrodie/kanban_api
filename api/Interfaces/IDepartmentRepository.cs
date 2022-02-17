using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace api.Repositories
{
    public interface IDepartmentRepository
    {
        Task<ICollection<DepartmentDTO>> GetDepartments();
        Task<Department> GetDepartment(int DepartmentId);
        Task<List<string>> GetDepartmentNames();
        Task<ICollection<Department>> Search(string searchQuery, List<int> departmentId);
        Task<bool> CreateDepartment(Department department, User user);
        Task<bool> PatchDepartment(Department department, JsonPatchDocument<Department> patchDoc);
        Task<bool> UpdateDepartment(Department department, List<int> userId);
        Task<bool> DeleteDepartment(Department department);
    }
}