namespace bggparser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DbGames", new[] { "Name" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.DbGames", "Name", unique: true);
        }
    }
}
