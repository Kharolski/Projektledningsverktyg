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
                        MemberId = c.Int(nullable: false),
                        ProjectId = c.Int(),
                        Member_Id = c.Int(),
                        Creator_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Members", t => t.Member_Id)
                .ForeignKey("dbo.Members", t => t.Creator_Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId)
                .Index(t => t.Member_Id)
                .Index(t => t.Creator_Id);
            
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
                        Event_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Members", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.Events", "Creator_Id", "dbo.Members");
            DropForeignKey("dbo.Tasks", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tasks", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Projects", "ProjectManagerId", "dbo.Members");
            DropForeignKey("dbo.ProjectMembers", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.ProjectMembers", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.Events", "Member_Id", "dbo.Members");
            DropIndex("dbo.ProjectMembers", new[] { "Member_Id" });
            DropIndex("dbo.ProjectMembers", new[] { "Project_Id" });
            DropIndex("dbo.Tasks", new[] { "ProjectId" });
            DropIndex("dbo.Tasks", new[] { "MemberId" });
            DropIndex("dbo.Projects", new[] { "ProjectManagerId" });
            DropIndex("dbo.Members", new[] { "Event_Id" });
            DropIndex("dbo.Events", new[] { "Creator_Id" });
            DropIndex("dbo.Events", new[] { "Member_Id" });
            DropIndex("dbo.Events", new[] { "ProjectId" });
            DropTable("dbo.ProjectMembers");
            DropTable("dbo.Tasks");
            DropTable("dbo.Projects");
            DropTable("dbo.Members");
            DropTable("dbo.Events");
        }
    }
}
