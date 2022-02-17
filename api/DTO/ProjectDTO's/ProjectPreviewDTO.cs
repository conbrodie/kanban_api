using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTO
{
    public class ProjectPreviewDTO
    {
        [Key] 
        public int ProjectId { get; set; }
        public String ProjectTitle { get; set; }
        public Boolean Approved { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public List<ProjectMemberDTO> ProjectMembers { get; set; }
        public List<ProjectDepartmentDTO> ProjectDepartments { get; set; }
    }
}