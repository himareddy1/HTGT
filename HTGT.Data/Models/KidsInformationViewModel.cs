using System.ComponentModel.DataAnnotations;

namespace HTGT.Data.Models
{
    public class KidsInformationIndexViewModel
    {
        public int SID { get; set; }
        public string KName { get; set; }
        public string ParentsName { get; set; }
        public int DayofBirth { get; set; }
        public int MonthofBirth { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string ReminderSentOn { get; set; }   
        public bool IsActive { get; set; }
    }

    public class KidsInformationCreateViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Parent Name")]
        public string ParentName { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date or Birth")]
        public System.DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
    }

    public class KidsInformationEditViewModel
    {
        public int SID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string KName { get; set; }

        [Required]
        [Display(Name = "Parent Name")]
        public string ParentsName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public System.DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
    }

    public class KidsInformationEmailModel
    {
        public int SID { get; set; }
        public string KName { get; set; }
        public string ParentsName { get; set; }
        public int DayofBirth { get; set; }
        public int MonthofBirth { get; set; }
        public string Email { get; set; }
    }
}