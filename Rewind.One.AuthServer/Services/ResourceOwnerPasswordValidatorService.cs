using IdentityServer4.Validation;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rewind.One.AuthServer.Services
{
    public class ResourceOwnerPasswordValidatorService : IResourceOwnerPasswordValidator
    {
        private readonly IAuthServerService _authServerService;
        public ResourceOwnerPasswordValidatorService(IAuthServerService authServerService)
        {
            _authServerService = authServerService;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            bool success = await _authServerService.ValidateUser(context.UserName, context.Password);
            if (success)
            {
                var user = await _authServerService.GetUserByUserName(context.UserName);
                context.Result = new GrantValidationResult(
                        subject: user.Id,
                        authenticationMethod: "custom",
                        claims: user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue))
                    );
            }
            else
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant);
            }
        }
    }
}
