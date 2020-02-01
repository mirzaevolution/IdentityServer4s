using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Rewind.One.AuthServer.Entities;
namespace Rewind.One.AuthServer.Services
{
    public interface IAuthServerService
    {
        Task<bool> AddUser(AppUser user);
        Task AddUserClaim(string userId, AppUserClaims userClaim);
        Task AddUserLogin(string userId, AppUserLogin userLogin);
        Task<AppUser> GetUserById(string userId);
        Task<AppUser> GetUserByUserName(string username);
        Task<IEnumerable<AppUserClaims>> GetClaimsByUserId(string userId);
        Task<IEnumerable<AppUserClaims>> GetClaimsByUserName(string username);
        Task<IEnumerable<AppUserLogin>> GetLoginsByUserId(string userId);
        Task<IEnumerable<AppUserLogin>> GetLoginsByUserName(string username);
        Task<bool> IsUserActive(string userId);
        Task<bool> ValidateUser(string username, string password);
        Task<bool> UserExists(string username);

    }
}
