using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace api.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Project title required.")]
        public String Title { get; set; }
        [Required(ErrorMessage = "Project start date required.")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Project approval status required.")]
        [DefaultValue(false)]
        public Boolean Approved { get; set; }
        public ICollection<Sprint> Sprints { get; set; }
        public ICollection<ProjectMember> Members { get; set; }
        public ICollection<ProjectDepartment> Departments { get; set; }
    }
}