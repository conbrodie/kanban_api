using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace api.Models
{
    public class Department
    {
        [Key] 
        public int DepartmentId { get; set; }
        public String DepartmentName { get; set; }
        public String DepartmentDescription { get; set; }
        public String Color { get; set; }
        public ICollection<DepartmentMember> DepartmentMembers { get; set; }
        public ICollection<ProjectDepartment> ProjectDepartments { get; set; }
    }
}