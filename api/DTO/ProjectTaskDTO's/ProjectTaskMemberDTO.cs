using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class ProjectTaskMemberDTO
    {
        [Key] 
        public int ProjectTaskMemberId { get; set; }
        public UserDTO User { get; set; }
    }
}