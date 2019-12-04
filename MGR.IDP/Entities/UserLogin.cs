using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MGR.IDP.Entities
{
    public class UserLogin
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SubjectId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
