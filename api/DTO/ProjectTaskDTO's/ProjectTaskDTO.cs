using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class ProjectTaskDTO
    {
       [Key] 
        public int ProjectTaskId { get; set; }
        public int ProjectSprintListId { get; set; }
        public String ProjectTaskTitle { get; set; }
        public String ProjectTaskDescription { get; set; }
        public String ProjectTaskPriority { get; set; }
        public DateTime ProjectTaskDateCreated { get; set; }
        public DateTime ProjectTaskDateDue { get; set; }
        public Decimal ProjectTaskPercentageCompleted { get; set; }
        public ICollection<ProjectTaskMemberDTO> ProjectTaskMembers { get; set; }
        public ICollection<ProjectTaskCategoryDTO> ProjectTaskCategories { get; set; }
        public ICollection<ProjectTaskSubTaskDTO> ProjectTaskSubTasks { get; set; }
        public ICollection<ProjectTaskCommentDTO> ProjectTaskComments { get; set; }
    }
}