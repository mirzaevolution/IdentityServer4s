using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MGR.IDP.Services
{
    public class MirzaCoreUserProfileService : IProfileService
    {
        private readonly IMirzaCoreRepository _repository;
        public MirzaCoreUserProfileService(IMirzaCoreRepository repository)
        {
            _repository = repository;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = await _repository.GetUserClaimsBySubjectId(subjectId);
            context.IssuedClaims = claimsForUser.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = await _repository.IsUserActive(subjectId);

        }
    }
}
