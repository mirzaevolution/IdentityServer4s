using System;
using System.Collections.Generic;

namespace Rewind.One.AuthServer.Entities
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public virtual IList<AppUserClaims> Claims { get; set; } = new List<AppUserClaims>();
        public virtual IList<AppUserLogin> Logins { get; set; } = new List<AppUserLogin>();
    }

}
