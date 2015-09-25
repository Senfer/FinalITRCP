namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Solves : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Solves",
                c => new
                    {
                        SolvesID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        UserID = c.String(),
                    })
                .PrimaryKey(t => t.SolvesID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Solves");
        }
    }
}
