using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace api.Models
{
    public class DepartmentMember
    {
        [Key] 
        public int DepartmentMemberId { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
        public Department Department { get; set; }
        public User User { get; set; }
    }
}