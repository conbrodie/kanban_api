using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class CardMemberDTO
    {
        [Key] 
        public int CardMemberId { get; set; }
        public int UserId { get; set; }
        public String FirstName  { get; set; }
        public String LastName  { get; set; }
    }
}