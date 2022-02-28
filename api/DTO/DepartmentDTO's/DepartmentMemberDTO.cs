using System;

namespace api 
{
    public class DepartmentMemberDTO 
    {
        public int DepartmentMemberId  { get; set; }
        public int UserId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
    }
}