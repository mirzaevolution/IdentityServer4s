using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string ReturnUrl { get; set; }
    }
}
