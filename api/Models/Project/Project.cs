using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace api.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        [BindRequired]
        public String Title { get; set; }
        [BindRequired]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [BindRequired]
        public Boolean Approved { get; set; }
        public ICollection<Sprint> Sprints { get; set; }
        public ICollection<ProjectMember> Members { get; set; }
        public ICollection<ProjectDepartment> Departments { get; set; }
    }
}