using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class ProjectTaskCommentVoteDTO
    {
        [Key] 
        public int ProjectTaskCommentVoteId { get; set; }
        public int ProjectTaskCommentId { get; set; }
        public UserDTO User { get; set; }
    }
}