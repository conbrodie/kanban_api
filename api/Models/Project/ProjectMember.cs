using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class ProjectMember
    {
        [Key] 
        public int ProjectMemberId { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Project Project { get; set; }
    }
}