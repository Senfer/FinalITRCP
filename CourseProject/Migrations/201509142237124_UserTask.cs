namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTask : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagsID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        TagText = c.String(),
                    })
                .PrimaryKey(t => t.TagsID);
            
            CreateTable(
                "dbo.UserTasks",
                c => new
                    {
                        UserTaskID = c.Int(nullable: false, identity: true),
                        TaskText = c.String(),
                        TaskRating = c.Int(nullable: false),
                        TaskCategory = c.String(),
                        TaskDifficulty = c.String(),
                        SolveCount = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTaskID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserTasks");
            DropTable("dbo.Tags");
        }
    }
}
