using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class SprintDTO
    {
        [Key] 
        public int SprintId { get; set; }
        public String SprintName  { get; set; }
    }
}