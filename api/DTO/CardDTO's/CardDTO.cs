using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class CardDTO
    {
        [Key] 
        public int CardId { get; set; }
        public String CardTitle { get; set; }
        public String CardDescription { get; set; }
        public String CardPriority { get; set; }
        public DateTime CardDateCreated { get; set; }
        public DateTime CardDateDue { get; set; }
        public Decimal CardPercentageCompleted { get; set; }
        public ICollection<CardMemberDTO> Members { get; set; }
        public ICollection<CardCommentDTO> Comments { get; set; }
    }
}