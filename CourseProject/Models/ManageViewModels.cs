using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;


namespace CourseProject.Models
{
    public class CabinetModel
    {
        public System.Collections.Generic.IEnumerable<ApplicationUser> Users { get; set; }
        public System.Collections.Generic.IEnumerable<UserTask> Tasks { get; set; }
    }

    public class AhcivementModel
    {
        public System.Collections.Generic.IEnumerable<Ahcivement> Ahcivements { get; set; }
    }
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    public class SolveTaskModel
    {
        public UserTask Task { get; set; }
        public IEnumerable<Tags> Tags { get; set; }
        public IEnumerable<Answers> Answers { get; set; }
        public IEnumerable<Comments> Comments { get; set; }
        public Dictionary<string, string> Nicknames { get; set; }
        public int Rating { get; set; }
        public string UserName { get; set; }
        public int Solved { get; set; }

    }

    public class ChangeTaskModel
    {
        public UserTask Task { get; set; }
        public IEnumerable<Tags> Tags { get; set; }
        public IEnumerable<Answers> Answers { get; set; }
    }

    public class CabinetViewModel
    {
        public IEnumerable<UserTask> Tasks { get; set; }
        public IEnumerable<Ahcivement> Achievements { get; set; }
        public ApplicationUser User { get; set; }
        public int SolveCount { get; set; }
        public int TaskCount { get; set; }
        public int Comments { get; set; }
    }


   
}