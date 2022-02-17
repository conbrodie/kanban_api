using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTO
{
    public class ProjectTaskCommentDTO
    {
        [Key] 
        public int ProjectTaskCommentId { get; set; }
        public String Description { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<CardCommentVote> ProjectTaskCommentVotes { get; set; }
        public UserDTO User { get; set; }
    }
}