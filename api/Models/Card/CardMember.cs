using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class CardMember
    {
        [Key] 
        public int CardMemberId { get; set; }
        public int CardId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Card Card { get; set; }
    }
}