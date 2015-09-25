namespace CourseProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Answers2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        AnswersID = c.Int(nullable: false, identity: true),
                        TaskID = c.Int(nullable: false),
                        AnswerText = c.String(),
                    })
                .PrimaryKey(t => t.AnswersID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Answers");
        }
    }
}
