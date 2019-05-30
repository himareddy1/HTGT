using System;
using System.ComponentModel.DataAnnotations;

namespace HTGT.Data.Models
{
    public class HTGTUsersViewModel
    {
        [Key]
        public int HTGTUserID { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }   
        public bool IsEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public string UserRoleName { get; set; }

        /// <summary>
        /// Exports a short string list of Id, Email, Name separated by |
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("|", new string[] { EmailAddress, FirstName, LastName });
        }

        public bool FromString(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return false;
            string[] vals = val.Split('|');

            if (vals.Length < 2)
                return false;

            EmailAddress = vals[0];
            FirstName = vals[1];
            LastName = vals[2];

            return true;
        }
    }

    public class HTGTUserValidationModel
    {
        public int HTGTUserID { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? TokenExpirationDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime LockoutEndDateUtc { get; set; }
        public int? AccessFailedCount { get; set; }
        public string UserRoleName { get; set; }
    }

    public class HTGTUsersCreateModel
    {
        [Key]
        [Required(ErrorMessage = "The Email field is required. ")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "The First Name field is required. ")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required. ")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}