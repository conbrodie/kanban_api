using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class SubTaskAssignee
    {
        [Key] 
        public int SubTaskAssigneeId { get; set; }
        public int SubTaskId { get; set; }
        public int UserId { get; set; }
        public SubTask SubTask { get; set; }
        public User User { get; set; }
    }
}