using System.Data.Entity.Migrations;
using Projektledningsverktyg.Data.Context;
using System.Data.Entity;
using System.Data.SQLite.EF6;
using SQLite.CodeFirst;
using System.Data.SQLite.EF6.Migrations;

namespace Projektledningsverktyg.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }
    }
}
