using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class SprintList
    {
        [Key] 
        public int SprintListId { get; set; }
        [Required(ErrorMessage = "Sprint ID required.")]
        public int SprintId { get; set; }
        [Required(ErrorMessage = "Sprint list name required.")]
        public String Name { get; set; }
        public ICollection<Card> Cards { get; set; }   
        public Sprint Sprint { get; set; }  
    }
}