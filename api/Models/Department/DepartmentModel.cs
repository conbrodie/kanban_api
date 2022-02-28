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
        [Required(ErrorMessage = "DepartmentId required.")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Department name required.")]
        public String DepartmentName { get; set; }
        public String DepartmentDescription { get; set; }
        public String Color { get; set; }
        public ICollection<DepartmentMember> Members { get; set; }
        public ICollection<ProjectDepartment> Projects { get; set; }
    }
}