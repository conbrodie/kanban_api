using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTO
{
    public class SprintListDTO
    {
        [Key] 
        public int SprintListId { get; set; }
        public String Name  { get; set; }
        public ICollection<CardDTO> Cards { get; set; }   
    }
}