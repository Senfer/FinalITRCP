namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Answers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserTasks", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserTasks", "UserID", c => c.Int(nullable: false));
        }
    }
}
