using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGR.IDP.Entities
{
    public class User
    {
        public string SubjectId { get; set; } = Guid.NewGuid().ToString();

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public virtual ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
    }
}
