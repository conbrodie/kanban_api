using System;
using api.Models;

namespace api.DTO
{
    public class ProjectDepartmentDTO
    {
        public int ProjectDepartmentId { get; set; }
        public int DepartmentId { get; set; }
        public String DepartmentName  { get; set; }
        public String Color  { get; set; }
    }
}