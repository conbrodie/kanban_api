using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Sprint
    {
        [Key] 
        public int SprintId { get; set; }
        public int ProjectId { get; set; }
        public String SprintName { get; set; }
        public ICollection<SprintList> SprintLists { get; set; }     
        public Project Project { get; set; }
    }
}