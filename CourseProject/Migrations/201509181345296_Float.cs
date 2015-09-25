namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Float : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserTasks", "TaskRating", c => c.Single(nullable: false));
            DropColumn("dbo.AspNetUsers", "Rating");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Rating", c => c.Int(nullable: false));
            AlterColumn("dbo.UserTasks", "TaskRating", c => c.Int(nullable: false));
        }
    }
}
