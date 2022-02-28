using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class SprintDTO
    {
        [Key] 
        public int sprintId { get; set; }
        public String sprintName  { get; set; }
        public ICollection<SprintListDTO> SprintLists { get; set; }   
    }
}