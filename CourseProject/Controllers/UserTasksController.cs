using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using CourseProject.Models;

namespace CourseProject.Controllers
{
    /*
    Для класса WebApiConfig может понадобиться внесение дополнительных изменений, чтобы добавить маршрут в этот контроллер. Объедините эти инструкции в методе Register класса WebApiConfig соответствующим образом. Обратите внимание, что в URL-адресах OData учитывается регистр символов.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using CourseProject.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<UserTask>("UserTasks");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    
    public class UserTasksController : ODataController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: odata/UserTasks
        [EnableQuery]
        public IQueryable<UserTask> GetUserTasks()
        {
            return db.Tasks;
        }

        // GET: odata/UserTasks(5)
        [EnableQuery]
        public SingleResult<UserTask> GetUserTask([FromODataUri] int key)
        {
            return SingleResult.Create(db.Tasks.Where(userTask => userTask.UserTaskID == key));
        }

        // PUT: odata/UserTasks(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<UserTask> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserTask userTask = db.Tasks.Find(key);
            if (userTask == null)
            {
                return NotFound();
            }

            patch.Put(userTask);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserTaskExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userTask);
        }

        // POST: odata/UserTasks
        public IHttpActionResult Post(UserTask userTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tasks.Add(userTask);
            db.SaveChanges();

            return Created(userTask);
        }

        // PATCH: odata/UserTasks(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<UserTask> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserTask userTask = db.Tasks.Find(key);
            if (userTask == null)
            {
                return NotFound();
            }

            patch.Patch(userTask);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserTaskExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userTask);
        }

        // DELETE: odata/UserTasks(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            UserTask userTask = db.Tasks.Find(key);
            if (userTask == null)
            {
                return NotFound();
            }

            db.Tasks.Remove(userTask);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserTaskExists(int key)
        {
            return db.Tasks.Count(e => e.UserTaskID == key) > 0;
        }
    }
}
