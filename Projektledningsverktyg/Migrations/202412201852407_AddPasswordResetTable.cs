namespace Projektledningsverktyg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPasswordResetTable : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PasswordResets");
        }
    }
}
