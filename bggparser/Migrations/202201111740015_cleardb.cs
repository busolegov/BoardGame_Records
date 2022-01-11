namespace bggparser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cleardb : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DbGames", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.DbGames", new[] { "Name" });
        }
    }
}
