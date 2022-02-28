using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTO
{
    public class CardCommentDTO
    {
        [Key] 
        public int CardCommentId { get; set; }
        public String Description { get; set; }
        public DateTime DateCreated { get; set; }
        public UserDTO User { get; set; }
    }
}