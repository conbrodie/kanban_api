using System;
using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public String FirstName  { get; set; }
        public String LastName  { get; set; }
    }
}