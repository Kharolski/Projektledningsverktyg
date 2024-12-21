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


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Project>()
                .HasRequired(p => p.ProjectManager)
                .WithMany()
                .HasForeignKey(p => p.ProjectManagerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Task>()
                .HasRequired(t => t.AssignedTo)
                .WithMany(m => m.Tasks)
                .HasForeignKey(t => t.MemberId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Task>()
                .HasRequired(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .WillCascadeOnDelete(false);
        }
    }
}
