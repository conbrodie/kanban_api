using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class ProjectTaskSubTaskDTO
    {
        [Key] 
        public int SubTaskId { get; set; }
        public String SubTaskDescription { get; set; }
        public Boolean SubTaskChecked { get; set; }
        public DateTime SubTaskDueDate { get; set; }
        public SubTaskAssigneeDTO SubTaskAssignee { get; set; }
    }
}