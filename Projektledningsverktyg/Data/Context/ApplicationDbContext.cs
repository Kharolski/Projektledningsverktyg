using System;
using System.IO;
using System.Data.Entity;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Migrations;
using System.Data.SQLite;
using System.Configuration;

namespace Projektledningsverktyg.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base(new SQLiteConnection()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["ProjektledningsDB"].ConnectionString
            }, true)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjektledningsDB.sqlite");
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Project relationships
            modelBuilder.Entity<Project>()
                .HasRequired(p => p.ProjectManager)
                .WithMany()
                .HasForeignKey(p => p.ProjectManagerId)
                .WillCascadeOnDelete(false);

            // Task relationships
            modelBuilder.Entity<Task>()
                .HasOptional(t => t.AssignedTo)
                .WithMany(m => m.Tasks)
                .HasForeignKey(t => t.MemberId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Task>()
                .HasOptional(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .WillCascadeOnDelete(false);

            // Event relationships
            modelBuilder.Entity<Event>()
                .HasRequired(e => e.Creator)
                .WithMany(m => m.CreatedEvents)
                .HasForeignKey(e => e.CreatorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Participants)
                .WithMany(m => m.ParticipatingEvents)
                .Map(m =>
                {
                    m.ToTable("EventParticipants");
                    m.MapLeftKey("EventId");
                    m.MapRightKey("MemberId");
                });

            // Comment configuration
            modelBuilder.Entity<Comment>()
                .ToTable("Comments")
                .HasKey(c => c.Id)
                .Ignore(c => c.Discussion)
                .Ignore(c => c.DiscussionId)
                .Ignore(c => c.Event)
                .Ignore(c => c.EventId)
                .Ignore(c => c.Chat)
                .Ignore(c => c.ChatId);

            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.Member)
                .WithMany()
                .HasForeignKey(c => c.MemberId);

            modelBuilder.Entity<Comment>()
                .HasOptional(c => c.Task)
                .WithMany()
                .HasForeignKey(c => c.TaskId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
