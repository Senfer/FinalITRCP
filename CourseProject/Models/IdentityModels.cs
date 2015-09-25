using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CourseProject.Models
{
    // Чтобы добавить данные профиля для пользователя, можно добавить дополнительные свойства в класс ApplicationUser. Дополнительные сведения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
        public float Rating { get; set; }
        public string NickName { get; set; }
<<<<<<< HEAD
        public string AhcievementMask { get; set; }
    }

    public class Ahcivement
    {
        public int AhcievementID { get; set; }
        public string AhcievementRU { get; set; }
        public string AhcievementUS { get; set; }
        public string AhcievementTextRU { get; set; }
        public string AhcievementTextUS { get; set; }
=======
        public int CountOfTasks { get; set; }
        public double CountOfSuccessfulAnswers { get; set; }
        public int CountOfComments { get; set; }
        public string AhcivementMask { get; set; }
>>>>>>> origin/master
    }

    public class Ahcivement
    {
        public int AhcivementID { get; set; }
        public string AhcivementRU { get; set; }
        public string AhcivementUS { get; set; }
        public string AhcivementTextRU { get; set; }
        public string AhcivementTextUS { get; set; }
        
    }
    public class UserTask
    {
        public int UserTaskID { get;set;}
        public string TaskName { get; set; }
        public string TaskText { get; set; }
        public float TaskRating { get; set; }
        public int TaskRatingCount { get; set; }
        public string TaskCategory { get; set; }
        public string TaskDifficulty { get; set; }
        public int SolveCount { get; set; }
        public string UserID { get; set; }
        public bool Deleted { get; set; }
    }

    public class Tags
    {
        public int TagsID { get; set; }
        public int TaskID { get; set; }
        public string TagText { get; set; }
    }

    public class Answers
    {
        public int AnswersID { get; set; }
        public int TaskID { get; set; }
        public string AnswerText { get; set; }
    }

    public class Solves
    {
        public int SolvesID { get; set; }
        public int TaskID { get; set; }
        public string UserID { get; set; }
    }

    public class Comments
    {
        public int CommentsID { get; set; }
        public int TaskID { get; set; }
        public string UserID { get; set; }
        public string CommentText { get; set; }
    }

    public class Ratings
    {
        public int RatingsID { get; set; }
        public int TaskID { get; set; }
        public string UserID { get; set; }
        public int RatingValue { get; set; }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public System.Data.Entity.DbSet<UserTask> Tasks { get; set; }
        public System.Data.Entity.DbSet<Tags> Tags { get; set; }
        public System.Data.Entity.DbSet<Answers> Answers { get; set; }
        public System.Data.Entity.DbSet<Solves> Solves { get; set; }
        public System.Data.Entity.DbSet<Comments> Comments { get; set; }
        public System.Data.Entity.DbSet<Ratings> Ratings { get; set; }
        public System.Data.Entity.DbSet<Ahcivement> Ahcivement { get; set; }
    }


}