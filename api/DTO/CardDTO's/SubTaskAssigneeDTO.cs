using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class SubTaskAssigneeDTO
    {
        [Key] 
        public int SubTaskAssigneeId { get; set; }
        public UserDTO User { get; set; }
    }
}