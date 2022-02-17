using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class CardCommentVote
    {
        [Key] 
        public int CardCommentVoteId { get; set; }
        public int CardCommentId { get; set; }
        public int UserId { get; set; }
        public CardComment CardComment { get; set; }
        public User User { get; set; }
    }
}