using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CourseProject.Models
{
    public class HomeViewModel 
    {
        public IEnumerable<UserTask> LatestTasks { get; set; }
        public IEnumerable<UserTask> RatedTasks { get; set; }
        public IEnumerable<UserTask> UnsolvedTasks { get; set; }
        public IEnumerable<ApplicationUser> RatedUsers { get; set; }
        public IEnumerable<Tags> Tags { get; set; }
    }

    public class FullTableModel
    {
        public System.Collections.Generic.IEnumerable<UserTask> Tasks { get; set; }
        public bool Reversed { get; set; }
        public string Sort { get; set; }
        public int Offset { get; set; }
        public int ActiveCount { get; set; }
    }

    
}
    
