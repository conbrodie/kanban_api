using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Card
    {
        [Key] 
        public int CardId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int SprintListId { get; set; }
        public int Order { get; set; }
        [Required(ErrorMessage = "Card title is required.")]
        public String CardTitle { get; set; }
        public String CardDescription { get; set; }
        public String CardPriority { get; set; }
        [Required(ErrorMessage = "Creation date required.")]
        public DateTime CardDateCreated { get; set; }
        public DateTime CardDateDue { get; set; }
        public Decimal CardPercentageCompleted { get; set; }
        public SprintList SprintList { get; set; }
        public ICollection<CardMember> Members { get; set; }
        public ICollection<SubTask> SubTasks { get; set; }
        public ICollection<CardComment> Comments { get; set; }
    }
}