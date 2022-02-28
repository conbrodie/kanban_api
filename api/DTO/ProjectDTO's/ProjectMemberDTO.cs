using System;
using api.Models;

namespace api.DTO
{
    public class ProjectMemberDTO
    {
        public int UserId { get; set; }
        public String FirstName  { get; set; }
        public String LastName  { get; set; }
    }
}