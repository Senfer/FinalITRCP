namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTasks4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTasks", "Deleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTasks", "Deleted");
        }
    }
}
