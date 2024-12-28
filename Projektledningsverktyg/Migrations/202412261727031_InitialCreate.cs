namespace Projektledningsverktyg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 2147483647),
                        Description = c.String(maxLength: 2147483647),
                        DateTime = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        ProjectId = c.Int(),
                        Member_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .ForeignKey("dbo.Members", t => t.CreatorId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.CreatorId)
                .Index(t => t.ProjectId)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 2147483647),
                        PasswordHash = c.String(maxLength: 2147483647),
                        FirstName = c.String(maxLength: 2147483647),
                        LastName = c.String(maxLength: 2147483647),
                        CreatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Role = c.String(maxLength: 2147483647),
                        BirthDate = c.DateTime(nullable: false),
                        ProfileImagePath = c.String(maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2147483647),
                        Description = c.String(maxLength: 2147483647),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        ProjectManagerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.ProjectManagerId)
                .Index(t => t.ProjectManagerId);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 2147483647),
                        Description = c.String(maxLength: 2147483647),
                        DueDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.MemberId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.MemberId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.PasswordResets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 2147483647),
                        Token = c.String(maxLength: 2147483647),
                        ExpirationDate = c.DateTime(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectMembers",
                c => new
                    {
                        Project_Id = c.Int(nullable: false),
                        Member_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Project_Id, t.Member_Id })
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.EventParticipants",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.MemberId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.MemberId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.EventParticipants", "MemberId", "dbo.Members");
            DropForeignKey("dbo.EventParticipants", "EventId", "dbo.Events");
            DropForeignKey("dbo.Events", "CreatorId", "dbo.Members");
            DropForeignKey("dbo.Tasks", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tasks", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Projects", "ProjectManagerId", "dbo.Members");
            DropForeignKey("dbo.ProjectMembers", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.ProjectMembers", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.Events", "Member_Id", "dbo.Members");
            DropIndex("dbo.EventParticipants", new[] { "MemberId" });
            DropIndex("dbo.EventParticipants", new[] { "EventId" });
            DropIndex("dbo.ProjectMembers", new[] { "Member_Id" });
            DropIndex("dbo.ProjectMembers", new[] { "Project_Id" });
            DropIndex("dbo.Tasks", new[] { "ProjectId" });
            DropIndex("dbo.Tasks", new[] { "MemberId" });
            DropIndex("dbo.Projects", new[] { "ProjectManagerId" });
            DropIndex("dbo.Events", new[] { "Member_Id" });
            DropIndex("dbo.Events", new[] { "ProjectId" });
            DropIndex("dbo.Events", new[] { "CreatorId" });
            DropTable("dbo.EventParticipants");
            DropTable("dbo.ProjectMembers");
            DropTable("dbo.PasswordResets");
            DropTable("dbo.Tasks");
            DropTable("dbo.Projects");
            DropTable("dbo.Members");
            DropTable("dbo.Events");
        }
    }
}
