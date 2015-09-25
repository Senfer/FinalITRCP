namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Achieve : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AhcievementMask", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AhcievementMask");
        }
    }
}
