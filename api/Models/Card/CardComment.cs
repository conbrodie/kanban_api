using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.DTO;

namespace api.Models
{
    public class CardComment
    {
        [Key] 
        public int CardCommentId { get; set; }
        public int CardId { get; set; }
        public int UserId { get; set; }
        public String Description { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<CardCommentVote> Votes { get; set; }
        public Card Card { get; set; }
        public User User { get; set; }
    }
}