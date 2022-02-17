using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    public class AppDbContext : IdentityDbContext<User, UserRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<DepartmentMember> DepartmentMembers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<SprintList> SprintLists { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardMember> CardMembers { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<SubTaskAssignee> SubTaskAssignees { get; set; }
        public DbSet<CardComment> CardComments { get; set; }
        public DbSet<CardCommentVote> CardCommentVotes { get; set; }
        public DbSet<ProjectDepartment> ProjectDepartments { get; set; }
          
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable(name:"Users");
                entity.Property(e => e.Id).HasColumnName("UserId");
            });

            builder.Entity<ProjectMember>()
                .HasOne(p => p.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(p=> p.ProjectId);
            builder.Entity<Sprint>()
                .HasOne(p => p.Project)
                .WithMany(p => p.Sprints)
                .HasForeignKey(p => p.ProjectId);
            builder.Entity<SprintList>()
                .HasOne(ps => ps.Sprint)
                .WithMany(psl => psl.SprintLists)
                .HasForeignKey(ps => ps.SprintId);
            builder.Entity<Card>()
                .HasOne(psl => psl.SprintList)
                .WithMany(pt => pt.Cards)
                .HasForeignKey(psl => psl.SprintListId);

            builder.Entity<ProjectDepartment>()
                .HasKey(x => x.ProjectDepartmentId);
            builder.Entity<ProjectDepartment>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Departments)
                .HasForeignKey(x => x.ProjectId);
            builder.Entity<ProjectDepartment>()
                .HasOne(x => x.Department)
                .WithMany(x => x.ProjectDepartments)
                .HasForeignKey(x => x.DepartmentId);

            builder.Entity<CardMember>()
               .HasKey(cardMember => cardMember.CardMemberId);
            builder.Entity<CardMember>()
                .HasOne(x => x.Card)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.CardId);
            builder.Entity<CardMember>()
                .HasOne(x => x.User)
                .WithMany(x => x.Cards)
                .HasForeignKey(x => x.UserId);

             builder.Entity<SubTask>()
                .HasOne(x => x.Card)
                .WithMany(x => x.SubTasks)
                .HasForeignKey(x => x.CardId);

             builder.Entity<SubTaskAssignee>()
                .HasOne(x => x.SubTask)
                .WithOne(x => x.SubTaskAssignee)
                .HasForeignKey<SubTaskAssignee>(x => x.SubTaskId);
             builder.Entity<SubTaskAssignee>()
                .HasOne(x => x.User)
                .WithOne(x => x.SubTaskAssignee)
                .HasForeignKey<SubTaskAssignee>(x => x.UserId);

             builder.Entity<CardComment>()
               .HasKey(x => x.CardCommentId);
            builder.Entity<CardComment>()
                .HasOne(x => x.Card)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.CardId);
            builder.Entity<CardComment>()
                .HasOne(x => x.User);

            builder.Entity<CardCommentVote>()
               .HasKey(x => x.CardCommentVoteId);
            builder.Entity<CardCommentVote>()
                .HasOne(x => x.CardComment)
                .WithMany(x => x.Votes)
                .HasForeignKey(x => x.CardCommentId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CardCommentVote>()
                .HasOne(x => x.User)
                .WithMany(x => x.Votes)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
