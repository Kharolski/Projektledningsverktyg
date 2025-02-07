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
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Household> Households { get; set; }    
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Comment> Comments { get; set; }


        // Our Meal setup
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealIngredient> MealIngredients { get; set; }
        public DbSet<MealDietaryTag> MealDietaryTags { get; set; }

        // Our Recipe setup
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Instruction> Instructions { get; set; }

        // Our Month/Week View setup
        public DbSet<MonthView> MonthViews { get; set; }
        public DbSet<DayView> DayViews { get; set; }


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

            // Comment relationships
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

            // Meal relationships
            modelBuilder.Entity<Meal>()
                .HasMany(m => m.Ingredients)
                .WithRequired(i => i.Meal)
                .HasForeignKey(i => i.MealId);

            modelBuilder.Entity<Meal>()
                .HasMany(m => m.DietaryTags)
                .WithRequired(t => t.Meal)
                .HasForeignKey(t => t.MealId);

            // Recipe relationships
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Ingredients)
                .WithRequired()
                .HasForeignKey(i => i.RecipeId);

            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Instructions)
                .WithRequired()
                .HasForeignKey(i => i.RecipeId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
