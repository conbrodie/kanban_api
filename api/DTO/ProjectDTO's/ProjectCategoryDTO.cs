using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class ProjectCategoryDTO
    {
        [Key] 
        public int ProjectCategoryId { get; set; }
        public String ProjectCategoryName  { get; set; }
        public String ProjectCategoryColor  { get; set; }
    }
}