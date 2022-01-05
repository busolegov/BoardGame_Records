namespace bggparser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DbGames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DbGameDates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        DbGameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DbGames", t => t.DbGameId, cascadeDelete: true)
                .Index(t => t.DbGameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DbGameDates", "DbGameId", "dbo.DbGames");
            DropIndex("dbo.DbGameDates", new[] { "DbGameId" });
            DropTable("dbo.DbGameDates");
            DropTable("dbo.DbGames");
        }
    }
}
