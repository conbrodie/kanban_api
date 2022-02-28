using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTO
{
    public class ProjectDTO
    {
        [Key] 
        public int ProjectId { get; set; }
        public String Title { get; set; }
        [DefaultValue(false)]
        public Boolean Approved { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<ProjectMemberDTO> Members { get; set; }
        public ICollection<ProjectDepartmentDTO> Departments { get; set; }
        public ICollection<SprintDTO> Sprints { get; set; }
    }
}