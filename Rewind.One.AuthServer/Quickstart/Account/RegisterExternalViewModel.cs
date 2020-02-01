using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterExternalViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Phone { get; set; }

        public string ReturnUrl { get; set; }

        public string Provider { get; set; }
    }
}
