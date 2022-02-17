using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class ProjectTaskCategoryDTO
    {
       [Key] 
        public int ProjectTaskCategoryId { get; set; }
        public ProjectCategoryDTO ProjectCategory { get; set; }
    }
}