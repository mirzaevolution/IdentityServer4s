using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rewind.One.AuthServer.Services
{
    public class AuthServerProfileDataService : IProfileService
    {
        private readonly IAuthServerService _authServerService;
        public AuthServerProfileDataService(IAuthServerService authServerService)
        {
            _authServerService = authServerService;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var id = context.Subject.GetSubjectId();
            var user = await _authServerService.GetUserById(id);
            context.IssuedClaims = user
                .Claims
                .Select(c => new Claim(c.ClaimType, c.ClaimValue))
                .ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var id = context.Subject.GetSubjectId();
            var user = await _authServerService.GetUserById(id);
            context.IsActive = user.IsActive;
        }
    }
}
