using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CourseProject.Models;


namespace CourseProject.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            CabinetViewModel Model = new CabinetViewModel();
            ApplicationDbContext DB = new ApplicationDbContext();
            Model.User = DB.Users.First(c => c.UserName == User.Identity.Name);
            Model.Tasks = DB.Tasks.Where(c => c.UserID == Model.User.Id);
            Model.TaskCount = Model.Tasks.ToList().Count;
            Model.SolveCount = DB.Solves.Count(c => c.UserID == Model.User.Id);
            Model.Comments = DB.Comments.Count(c => c.UserID == Model.User.Id);
            return View(Model);
        }

        //
        // GET: /Manage/RemoveLogin
        public ActionResult RemoveLogin()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return View(linkedAccounts);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Создание и отправка маркера
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Ваш код безопасности: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Отправка SMS через поставщик SMS для проверки номера телефона
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // Это сообщение означает наличие ошибки; повторное отображение формы
            ModelState.AddModelError("", "Не удалось проверить телефон");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Внешнее имя входа удалено."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Запрос перенаправления к внешнему поставщику входа для связывания имени входа текущего пользователя
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        public ActionResult AddingTasks(System.Collections.Generic.List<string> Answers, System.Collections.Generic.List<string> Tags, string Category, string Difficulty, string HTML, string Name, int? id)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            if (Answers != null && Difficulty != null && Category != null && HTML != null)
            {
                if (Answers.Count > 0)
                {
                    if (id == null)
                    {                        
                        UserTask NewTask = new UserTask();
                        NewTask.TaskName = Name;
                        NewTask.TaskDifficulty = Difficulty;
                        NewTask.TaskCategory = Category;
                        NewTask.TaskText = HTML;
                        NewTask.TaskRating = 0;
                        NewTask.SolveCount = 0;
                        NewTask.UserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
                        DB.Tasks.Add(NewTask);
                        DB.Entry(NewTask).State = System.Data.Entity.EntityState.Added;
                        int NewTaskID = DB.Tasks.ToList().Last(c => c.UserTaskID > 0).UserTaskID + 1;
                        for (int i = 0; i < Tags.Count; i++)
                        {
                            string CurrentTag = Tags[i];
                            Tags NewTag = new Tags();
                            NewTag.TagText = Tags[i];
                            NewTag.TaskID = NewTaskID;
                            DB.Tags.Add(NewTag);
                        }
                        for (int i = 0; i < Answers.Count; i++)
                        {
                            string CurrentAnswer = Answers[i];

                            Answers NewAnswer = new Answers();
                            NewAnswer.AnswerText = Answers[i];
                            NewAnswer.TaskID = NewTaskID;
                            DB.Answers.Add(NewAnswer);
                            DB.Entry(NewAnswer).State = System.Data.Entity.EntityState.Added;

                        }

                    }
                    else
                    {
                        DB.Tags.RemoveRange(DB.Tags.Where(c => c.TaskID == id));
                        DB.Answers.RemoveRange(DB.Answers.Where(c => c.TaskID == id));
                        for (int i = 0; i < Tags.Count; i++)
                        {
                            string CurrentTag = Tags[i];

                            Tags NewTag = new Tags();
                            NewTag.TagText = Tags[i];
                            NewTag.TaskID = (int)id;
                            DB.Tags.Add(NewTag);
                            DB.Entry(NewTag).State = System.Data.Entity.EntityState.Added;
                        }

                        for (int i = 0; i < Answers.Count; i++)
                        {
                            string CurrentAnswer = Answers[i];

                            Answers NewAnswer = new Answers();
                            NewAnswer.AnswerText = Answers[i];
                            NewAnswer.TaskID = (int)id;
                            DB.Answers.Add(NewAnswer);
                            DB.Entry(NewAnswer).State = System.Data.Entity.EntityState.Added;
                        }
                        DB.Tasks.First(c=>c.UserTaskID == id).TaskName = Name;
                        DB.Tasks.First(c => c.UserTaskID == id).TaskDifficulty = Difficulty;
                        DB.Tasks.First(c => c.UserTaskID == id).TaskCategory = Category;
                        DB.Tasks.First(c => c.UserTaskID == id).TaskText = HTML;                        
                    }
                    DB.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Home");
        }


        public ActionResult SolvingTask(int id, string Answer)
        {
            SolveTaskModel Model = new SolveTaskModel();
            ApplicationDbContext DB = new ApplicationDbContext();

            Model.Nicknames = new System.Collections.Generic.Dictionary<string,string>();
            Model.Task = DB.Tasks.First(c => c.UserTaskID == id);
            Model.Answers = DB.Answers.Where(c => c.TaskID == id).AsEnumerable();
            Model.Tags = DB.Tags.Where(c => c.TaskID == id).AsEnumerable();
            Model.UserName = DB.Users.First(c => c.Id == Model.Task.UserID).UserName;
            Model.Comments = DB.Comments.Where(c => c.TaskID == id).AsEnumerable();

            System.Collections.Generic.List<string> IDs = new System.Collections.Generic.List<string>();
            foreach(var i in DB.Comments.Where(c => c.TaskID == id))
                IDs.Add(i.UserID);
            IDs = IDs.Distinct().ToList();

            foreach (var i in IDs)
            {
                Model.Nicknames.Add(i, DB.Users.First(c=>c.Id==i).NickName);
            }
            
            string CurrentUserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
            Model.Rating = 0;
            if (DB.Ratings.Any(c => c.UserID == CurrentUserID && c.TaskID == id)==true)
                Model.Rating = DB.Ratings.First(c => c.UserID == CurrentUserID && c.TaskID == id).RatingValue;
            
            Model.Solved = -1;
            if (DB.Tasks.Any(c => c.UserID == CurrentUserID && c.UserTaskID == id) == true)
                Model.Solved = -2;
            else
            {
                if (DB.Solves.Any(c => c.UserID == CurrentUserID && c.TaskID == id) == true)
                {
                    Model.Solved = -3;
                }
                else
                {
                    if (Answer != "" && Answer != null)
                    {
                        if (DB.Answers.Where(c => (c.TaskID == id) && (c.AnswerText == Answer)).ToList().Count != 0)
                        {
                            Model.Solved = 1;
                            Solves NewSolve = new Solves();
                            NewSolve.TaskID = id;
                            NewSolve.UserID = CurrentUserID;
                            DB.Solves.Add(NewSolve);
                            DB.Tasks.First(c => c.UserTaskID == id).SolveCount++;
                            DB.Entry(NewSolve).State = System.Data.Entity.EntityState.Added;
                            DB.SaveChanges();
                        }
                        else
                            Model.Solved = 0;
                    }
                }

            }
            return View(Model);
        }

        public ActionResult AddComment(string Comment, int id)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            Comments NewComment = new Comments();
            NewComment.CommentText = Comment;
            NewComment.TaskID = id;
            NewComment.UserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
            DB.Entry(NewComment).State = System.Data.Entity.EntityState.Added;
            DB.SaveChanges();
            return RedirectToAction("SolvingTask", new { id = id });
        }

        public ActionResult Vote(int Rating, int id)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            string CurrentUserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
            string TaskCreatorID = DB.Tasks.First(c => c.UserTaskID == id).UserID;

            
            
            if (DB.Ratings.Any(c => c.TaskID == id && c.UserID == CurrentUserID) == false)
            {
                DB.Tasks.First(c => c.UserTaskID == id).TaskRatingCount++;
                Ratings NewRating = new Ratings();
                NewRating.RatingValue = Rating;
                NewRating.TaskID = id;
                NewRating.UserID = CurrentUserID;
                DB.Entry(NewRating).State = System.Data.Entity.EntityState.Added;

            }
            else
            {
                DB.Ratings.First(c => c.UserID == CurrentUserID && c.TaskID == id).RatingValue = Rating;
            }
    
            int Summary = 0;
            foreach(var i in DB.Ratings.Where(c=>c.TaskID == id))
                Summary += i.RatingValue;
            int TaskRating = Summary/DB.Ratings.Count(c=>c.TaskID==id);
            DB.Tasks.First(c => c.UserTaskID == id).TaskRating = TaskRating;

            float UserRating = 0;
            foreach (var i in DB.Tasks.Where(c => c.UserID == TaskCreatorID))
                if(i.TaskRatingCount > 0)
                    UserRating += (i.TaskRating/i.TaskRatingCount);
            int RatingCount = DB.Tasks.Count(c => c.UserID == TaskCreatorID);
            UserRating = UserRating / RatingCount;
            DB.Users.First(c => c.Id == TaskCreatorID).Rating = UserRating;
            DB.SaveChanges();
            return RedirectToAction("SolvingTask", new { id = id });
        }

        public ActionResult ChangeTask(int id)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            ChangeTaskModel Model = new ChangeTaskModel();
            Model.Task = DB.Tasks.First(c => c.UserTaskID == id);
            Model.Answers = DB.Answers.Where(c => c.TaskID == id);
            Model.Tags = DB.Tags.Where(c=>c.TaskID == id);
            return View(Model);
        }


        public ActionResult DeleteTask(int id)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            DB.Tasks.First(c => c.UserTaskID == id).Deleted = true;
            DB.Tags.RemoveRange(DB.Tags.Where(c => c.TaskID == id).AsEnumerable());
            DB.Answers.RemoveRange(DB.Answers.Where(c => c.TaskID == id).AsEnumerable());
            DB.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

       
#region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}