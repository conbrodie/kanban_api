using System;
using System.Collections.Generic;
using api.DTO;

namespace api.Models
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public String DepartmentName { get; set; }
        public String DepartmentDescription { get; set; }
        public String Color { get; set; }
        public ICollection<DepartmentMemberDTO> Members { get; set; }
    }
}