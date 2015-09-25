using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using MoreLinq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CourseProject.Models;
<<<<<<< HEAD
using MarkdownSharp;
using Lucene.Net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

using Version = Lucene.Net.Util.Version;



=======
//using MarkdownSharp;
>>>>>>> origin/master

namespace CourseProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            HomeViewModel Model = new HomeViewModel();
            int Length = 10;
            if (DB.Tasks.Any(c => c.Deleted == false))
            {
                List<UserTask> Active = DB.Tasks.Where(c => c.Deleted == false).ToList();

                List<UserTask> Reversed = Active;
                Reversed.Reverse();
                if (Reversed.Count < 10)
                    Length = Reversed.Count;
                Model.LatestTasks = Reversed.Take(Length);

                Length = 10;
                List<UserTask> Rated = Active.Where(c => c.TaskRatingCount > 0).ToList();
                if (Rated.OrderByDescending(c => c.TaskRating / c.TaskRatingCount).ToList().Count < 10)
                    Length = Rated.OrderByDescending(c => c.TaskRating / c.TaskRatingCount).ToList().Count;
                Model.RatedTasks = Rated.OrderByDescending(c => c.TaskRating / c.TaskRatingCount).Take(Length);


                Length = 10;
                if (Active.Where(c => c.SolveCount == 0).ToList().Count < 10)
                    Length = Active.Where(c => c.SolveCount == 0).ToList().Count;
                Model.UnsolvedTasks = Active.Where(c => c.SolveCount == 0).Take(Length);


                Length = 10;
                System.Collections.Generic.IEnumerable<ApplicationUser> RatedUsers = DB.Users.Where(c => c.Rating > 0);
                if (RatedUsers.OrderByDescending(c => c.Rating).ToList().Count < 10)
                    Length = RatedUsers.OrderByDescending(c => c.Rating).ToList().Count;

                Model.RatedUsers = RatedUsers.OrderByDescending(c => c.Rating).Take(Length);


            }
<<<<<<< HEAD
            if (Request.IsAuthenticated == true)
            {
                string CurrentUserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
                HttpCookie NewCookie = new HttpCookie("Nickname");
                NewCookie.Value = DB.Users.First(c => c.Id == CurrentUserID).NickName;
                HttpContext.Response.Cookies.Add(NewCookie);
            }
            if (DB.Tags.ToList().Count > 0)
            {
                Model.Tags = DB.Tags.AsEnumerable();
                Model.Tags = Model.Tags.DistinctBy(c => c.TagText.ToLower());
            }

=======
            string CurrentUserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
            HttpCookie NewCookie = new HttpCookie("Nickname");
            NewCookie.Value = DB.Users.First(c => c.Id == CurrentUserID).NickName;
            HttpContext.Response.Cookies.Add(NewCookie);
>>>>>>> origin/master
            return View(Model);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FullTable(string Tag, string Sort, bool? Reversed, int? Offset, bool? search, string SearchString)
        {
            FullTableModel Model = new FullTableModel();
            Model.Tasks = new List<UserTask>();
            List<UserTask> TempModel = new List<UserTask>();
            if (search == null)
            {
                ApplicationDbContext DB = new ApplicationDbContext();
                List<UserTask> Active = DB.Tasks.Where(c => c.Deleted == false).ToList();
                TempModel = Active;
                if (Tag != null && Tag != "")
                {
                    List<UserTask> Temp = new List<UserTask>();
                    foreach (var i in DB.Tags.Where(c => c.TagText.ToLower() == Tag.ToLower()).AsEnumerable())

                        Temp.Add(Active.First(c => c.UserTaskID == i.TaskID));
                    TempModel = Temp;
                }
                Model.Sort = "Name";
                if (Sort != null && Tag != "")
                {
                    if (Sort == "Name")
                        TempModel = Active.OrderBy(c => c.TaskName).ToList();
                    if (Sort == "Category")
                        TempModel= Active.OrderBy(c => c.TaskCategory).ToList();
                    if (Sort == "Difficulty")
                        TempModel = Active.OrderBy(c => c.TaskDifficulty).ToList();
                    if (Sort == "Rating")
                    {
                        List<UserTask> Rated = Active.Where(c => c.TaskRatingCount > 0).OrderBy(c => c.TaskRating / c.TaskRatingCount).ToList();
                        List<UserTask> UnRated = Active.Where(c => c.TaskRatingCount == 0).ToList();
                        Rated.InsertRange(0, UnRated);
                        TempModel = Rated;
                    }
                    Model.Sort = Sort;
                }
                Model.Reversed = false;
                if (Reversed != null)
                {
                    Model.Reversed = (bool)Reversed;
                    if (Reversed == true)
                    {
                        TempModel.Reverse();
                    }
                }
            }
            else
            {
                Model.Offset = 0;
                ApplicationDbContext DB = new ApplicationDbContext();
                List<UserTask> SearchList = LuceneSearch.SearchDefault(SearchString).ToList();
                List<UserTask> NewList = new List<UserTask>();
                foreach (var i in SearchList)
                    NewList.Add(DB.Tasks.First(c => c.UserTaskID == i.UserTaskID));
                TempModel = NewList;
                Model.Reversed = false;
                Model.Sort = "Name";
            }
            Model.ActiveCount = TempModel.ToList().Count;

            Model.Tasks = TempModel.Take(10);
            if (Offset != null && Offset * 10 < TempModel.Count && Offset > 0)
            {
                Model.Offset = (int)Offset;
                TempModel.RemoveRange(0, 10 * (int)Offset);
                Model.Tasks = TempModel.Take(10);
            }
            else
                if (Offset * 10 > TempModel.Count || Offset < 0)
                    Model.Offset = 0;
            return View(Model);
        }




        [HttpGet]
        public string CompanionGetAllTasks()
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            List<UserTask> Temp = DB.Tasks.Where(c => c.UserTaskID > 0).ToList();




            XmlSerializer xsSubmit = new XmlSerializer(typeof(List<UserTask>));
            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, Temp);
                var xml = sww.ToString();
                return xml;
            }

        }


        [HttpGet]
        public string CompanionGetTask(int id, string UserID)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            UserTask Temp = DB.Tasks.First(c => c.UserTaskID == id);
            if (UserID == Temp.UserID)
                return "YOU CREATED THIS TASK";
            else
            {
                if (DB.Solves.Any(c => c.TaskID == id && c.UserID == UserID) == true)
                    return "YOU ALREADY SOLVED THIS";
                else
                {
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(UserTask));
                    using (StringWriter sww = new StringWriter())
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, Temp);
                        var xml = sww.ToString();
                        return xml;
                    }
                }
            }

        }

        [HttpGet]
        public string CompanionSolvingTask(string Answer, int id, string UserID)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            if (DB.Answers.Any(c => c.AnswerText == Answer && c.TaskID == id) == true)
            {
                if (Answer != "" && Answer != null)
                {
                    if (DB.Answers.Where(c => (c.TaskID == id) && (c.AnswerText == Answer)).ToList().Count != 0)
                    {
                        Solves NewSolve = new Solves();
                        NewSolve.TaskID = id;
                        NewSolve.UserID = UserID;
                        DB.Solves.Add(NewSolve);
                        DB.Tasks.First(c => c.UserTaskID == id).SolveCount++;
                        DB.Entry(NewSolve).State = System.Data.Entity.EntityState.Added;
                        DB.SaveChanges();
                    }
                }
                return "success";
            }
            else
                return "fail";
        }


        [HttpPost]
        public void ChangeNickname(string value)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            string CurrentUserID = DB.Users.First(c => c.UserName == User.Identity.Name).Id;
            DB.Users.First(c => c.Id == CurrentUserID).NickName = value;
            DB.SaveChanges();
            Response.Cookies["Nickname"].Value = value;
        }

       /* [HttpGet]
        public string CompanionNeedsView(int id)
        {
            ApplicationDbContext DB = new ApplicationDbContext();
            Markdown translator = new Markdown();
            return translator.Transform(DB.Tasks.First(c => c.UserTaskID == id).TaskText);
<<<<<<< HEAD

        }


        public ActionResult UpdateSearch()
        {


            LuceneSearch.AddUpdateLuceneIndex(DataRepository.GetAll());
            
            //var new_record = new UserTask { Id = X, Name = "SomeName", Description = "SomeDescription" };
            //LuceneSearch.AddUpdateLuceneIndex(new_record);

            return RedirectToAction("Index");
        }

       


=======
 
        }  */
        
>>>>>>> origin/master
    }


}