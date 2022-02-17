using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class SubTask
    {
        [Key] 
        public int SubTaskId { get; set; }
        public int CardId { get; set; }
        public String SubTaskDescription { get; set; }
        public Boolean SubTaskCompleted { get; set; }
        public DateTime SubTaskDueDate { get; set; }
        public Card Card { get; set; }
        public SubTaskAssignee SubTaskAssignee { get; set; }
    }
}