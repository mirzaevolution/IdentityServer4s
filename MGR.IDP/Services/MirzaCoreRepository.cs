using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using MGR.IDP.Entities;
using Microsoft.EntityFrameworkCore;

namespace MGR.IDP.Services
{
    public class MirzaCoreRepository : IMirzaCoreRepository
    {
        private readonly MirzaCoreDbContext _context;
        public MirzaCoreRepository(MirzaCoreDbContext context)
        {
            _context = context;
        }
        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task AddUserClaim(string subjectId, string claimType, string claimValue)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.SubjectId == subjectId);
            if (user != null)
            {
                user.Claims.Add(new UserClaim(claimType, claimValue)
                {
                    SubjectId = subjectId
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public async Task AddUserLogin(string subjectId, string loginProvider, string providerKey)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.SubjectId == subjectId);
            if (user != null)
            {
                user.Logins.Add(new UserLogin
                {
                    SubjectId = subjectId,
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public async Task<User> GetUserByProvider(string loginProvider, string providerKey)
        {
            return await _context.Users.Include(c=>c.Claims).Include(c=>c.Logins).FirstOrDefaultAsync(c => c.Logins.Any(l => l.LoginProvider.Equals(loginProvider) && l.ProviderKey.Equals(providerKey)));
        }

        public async Task<User> GetUserBySubjectIdAsync(string subjectId)
        {
            return await _context.Users.Include(c=>c.Claims).Include(c=>c.Logins).FirstOrDefaultAsync(c => c.SubjectId.Equals(subjectId));
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.Include(c=>c.Claims).Include(c => c.Logins).FirstOrDefaultAsync(c => c.UserName.Equals(username));
        }

        public async Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectId(string subjectId)
        {
            var user = await _context.Users.Include(c => c.Claims).FirstOrDefaultAsync(c => c.SubjectId.Equals(subjectId));
            if (user == null)
                return new List<UserClaim>();
            return user.Claims.ToList();
        }

        public async Task<IEnumerable<UserLogin>> GetUserLoginsBySubjectId(string subjectId)
        {
            var user = await _context.Users.Include(c => c.Logins).FirstOrDefaultAsync(c => c.SubjectId.Equals(subjectId));
            if (user == null)
                return new List<UserLogin>();
            return user.Logins.ToList();
        }

        public async Task<bool> IsCredentialValidAsync(string username, string password)
        {
            string hashedPassword = password.Sha256();
            return (await _context.Users.FirstOrDefaultAsync(c => c.UserName.Equals(username) && c.Password.Equals(hashedPassword))) !=null;
        }

        public async Task<bool> IsUserActive(string subjectId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.SubjectId.Equals(subjectId));
            if (user == null)
                return false;
            return user.IsActive;
        }
    }
}
