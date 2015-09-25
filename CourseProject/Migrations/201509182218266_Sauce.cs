namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sauce : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingsID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        UserID = c.String(),
                        RatingValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RatingsID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Ratings");
        }
    }
}
