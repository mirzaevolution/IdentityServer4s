using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
namespace Rewind.One.AuthServer.Entities
{
    public class AuthServerContext : DbContext
    {
        public AuthServerContext(DbContextOptions<AuthServerContext> options) : base(options)
        {
        }
        public virtual DbSet<AppUser> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>(options =>
            {
                options.HasIndex(c => c.UserName).HasName("Idx_Users_UserName");
                options.HasMany(c => c.Claims).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
                options.HasMany(c => c.Logins).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
                options.ToTable("Users");

            });
            modelBuilder.Entity<AppUserClaims>(options =>
            {
                options.HasIndex(c => c.UserId).HasName("Idx_Claims_UserId");
                options.ToTable("Claims");
            });
            modelBuilder.Entity<AppUserLogin>(options =>
            {
                options.HasIndex(c => c.UserId).HasName("Idx_Logins_UserId");
                options.ToTable("Logins");
            });

        }
    }
}
