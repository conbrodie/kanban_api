using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<DepartmentMember> Departments { get; set; }
        public ICollection<CardMember> Cards { get; set; }
        public SubTaskAssignee SubTaskAssignee { get; set; }
        public ICollection<CardComment> Comments { get; set; }
        public ICollection<CardCommentVote> Votes { get; set; }
    }
}

