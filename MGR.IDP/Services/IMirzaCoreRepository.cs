using MGR.IDP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGR.IDP.Services
{
    public interface IMirzaCoreRepository
    {
        Task<User> GetUserByUserNameAsync(string username);
        Task<User> GetUserBySubjectIdAsync(string subjectId);
        Task<User> GetUserByProvider(string loginProvider, string providerKey);
        Task<IEnumerable<UserLogin>> GetUserLoginsBySubjectId(string subjectId);
        Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectId(string subjectId);
        Task<bool> IsCredentialValidAsync(string username, string password);
        Task<bool> IsUserActive(string subjectId);
        Task<User> AddUser(User user);
        Task AddUserLogin(string subjectId, string loginProvider, string providerKey);
        Task AddUserClaim(string subjectId, string claimType, string claimValue);
    }
}
