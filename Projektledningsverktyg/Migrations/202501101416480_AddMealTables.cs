namespace Projektledningsverktyg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMealTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MealDietaryTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MealId = c.Int(nullable: false),
                        TagName = c.String(maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Meals", t => t.MealId, cascadeDelete: true)
                .Index(t => t.MealId);
            
            CreateTable(
                "dbo.Meals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2147483647),
                        Type = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Calories = c.Int(nullable: false),
                        Protein = c.Double(nullable: false, storeType: "real"),
                        Carbohydrates = c.Double(nullable: false, storeType: "real"),
                        Fat = c.Double(nullable: false, storeType: "real"),
                        Fiber = c.Double(nullable: false, storeType: "real"),
                        Instructions = c.String(maxLength: 2147483647),
                        PreparationTime = c.Int(nullable: false),
                        ServingSize = c.Int(nullable: false),
                        Notes = c.String(maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MealIngredients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MealId = c.Int(nullable: false),
                        Name = c.String(maxLength: 2147483647),
                        Amount = c.String(maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Meals", t => t.MealId, cascadeDelete: true)
                .Index(t => t.MealId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MealIngredients", "MealId", "dbo.Meals");
            DropForeignKey("dbo.MealDietaryTags", "MealId", "dbo.Meals");
            DropIndex("dbo.MealIngredients", new[] { "MealId" });
            DropIndex("dbo.MealDietaryTags", new[] { "MealId" });
            DropTable("dbo.MealIngredients");
            DropTable("dbo.Meals");
            DropTable("dbo.MealDietaryTags");
        }
    }
}
