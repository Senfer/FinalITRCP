namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentsID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        UserID = c.String(),
                        CommentText = c.String(),
                    })
                .PrimaryKey(t => t.CommentsID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Comments");
        }
    }
}
