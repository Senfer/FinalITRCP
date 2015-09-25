namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Answers3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTasks", "TaskName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTasks", "TaskName");
        }
    }
}
