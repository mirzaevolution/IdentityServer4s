using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace MGR.IDP.Entities
{
    public class MirzaCoreDbContext : DbContext
    {
        public MirzaCoreDbContext(DbContextOptions<MirzaCoreDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(options =>
            {
                options.HasKey(c => c.SubjectId);
                options.Property(c => c.UserName).IsRequired();
                options.Property(c => c.Password).IsRequired();
                options.HasMany(c => c.Claims);
                options.HasMany(c => c.Logins);
                options.ToTable("Users");
            });
            modelBuilder.Entity<UserClaim>(options =>
            {
                options.HasKey(c => c.Id);
                options.Property(c => c.SubjectId).IsRequired();
                options.Property(c => c.ClaimType).IsRequired();
                options.Property(c => c.ClaimValue).IsRequired();
                options.ToTable("UserClaims");
            });
            modelBuilder.Entity<UserLogin>(options =>
            {
                options.HasKey(c => c.Id);
                options.Property(c => c.SubjectId).IsRequired();
                options.ToTable("UserLogins");
            });
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserClaim> Claims { get; set; }
        public virtual DbSet<UserLogin> Logins { get; set; }
    }
}
