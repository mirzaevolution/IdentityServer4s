using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rewind.One.AuthServer.Entities;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.Models;

namespace Rewind.One.AuthServer.Services
{
    public class AuthServerService : IAuthServerService
    {
        private readonly AuthServerContext _context;
        public AuthServerService(AuthServerContext context)
        {
            _context = context;
        }

        public async Task AddUser(AppUser user)
        {
            _context.Add(user);
            var total = await _context.SaveChangesAsync();
        }

        public async Task AddUserClaim(string userId, AppUserClaims userClaim)
        {
            var user = await _context.Users.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.Claims.Add(userClaim);
            _context.Users.Add(user);
            var total = await _context.SaveChangesAsync();
        }

        public async Task AddUserLogin(string userId, AppUserLogin userLogin)
        {

            var user = await _context.Users.Include(c => c.Logins).FirstOrDefaultAsync(c => c.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.Logins.Add(userLogin);
            _context.Users.Add(user);
            var total = await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppUserClaims>> GetClaimsByUserId(string userId)
        {

            var user = await _context.Users.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            var claims = user.Claims.ToList();
            return claims;
        }

        public async Task<IEnumerable<AppUserClaims>> GetClaimsByUserName(string username)
        {
            var user = await _context.Users.Include(c => c.Claims).FirstOrDefaultAsync(c => c.UserName == username);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            var claims = user.Claims.ToList();
            return claims;
        }

        public async Task<IEnumerable<AppUserLogin>> GetLoginsByUserId(string userId)
        {
            var user = await _context.Users.Include(c => c.Logins).FirstOrDefaultAsync(c => c.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            var logins = user.Logins.ToList();
            return logins;
        }

        public async Task<IEnumerable<AppUserLogin>> GetLoginsByUserName(string username)
        {
            var user = await _context.Users.Include(c => c.Logins).FirstOrDefaultAsync(c => c.UserName == username);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            var logins = user.Logins.ToList();
            return logins;
        }

        public async Task<AppUser> GetUserById(string userId)
        {

            var user = await _context.Users.Include(c => c.Logins).Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task<AppUser> GetUserByUserName(string username)
        {

            var user = await _context.Users.Include(c => c.Logins).Include(c => c.Claims).FirstOrDefaultAsync(c => c.UserName == username);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task<bool> IsUserActive(string userId)
        {

            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user.IsActive;
        }

        public async Task<bool> UserExists(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.UserName.ToLower() == username.ToLower());
            return user != null ? true : false;
        }

        public async Task<bool> ValidateUser(string username, string password)
        {
            string hashedPassword = password.Sha256();

            var user = await _context.Users.FirstOrDefaultAsync(c => c.UserName.ToLower() == username.ToLower());

            if (user == null)
            {
                return false;
            }
            var valid = user.Password.Equals(hashedPassword);
            return valid;
        }
    }
}
