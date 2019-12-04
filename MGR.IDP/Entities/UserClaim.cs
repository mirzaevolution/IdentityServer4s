using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MGR.IDP.Entities
{
    public class UserClaim
    {
        public UserClaim()
        {

        }
        public UserClaim(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SubjectId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
