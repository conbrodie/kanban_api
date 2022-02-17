using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class ProjectDepartment
    {
        [Key] 
        public int ProjectDepartmentId { get; set; }
        public int DepartmentId { get; set; }
        public int ProjectId { get; set; }
        public Department Department { get; set; }
        public Project Project { get; set; }
    }
}