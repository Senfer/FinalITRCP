namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Users11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTasks", "TaskRatingCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTasks", "TaskRatingCount");
        }
    }
}
