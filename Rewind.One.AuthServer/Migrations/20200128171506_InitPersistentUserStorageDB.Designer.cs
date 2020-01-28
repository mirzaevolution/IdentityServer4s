﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rewind.One.AuthServer.Entities;

namespace Rewind.One.AuthServer.Migrations
{
    [DbContext(typeof(AuthServerContext))]
    [Migration("20200128171506_InitPersistentUserStorageDB")]
    partial class InitPersistentUserStorageDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Rewind.One.AuthServer.Entities.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .HasName("Idx_Users_UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Rewind.One.AuthServer.Entities.AppUserClaims", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AppUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("UserId")
                        .HasName("Idx_Claims_UserId");

                    b.ToTable("Claims");
                });

            modelBuilder.Entity("Rewind.One.AuthServer.Entities.AppUserLogin", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AppUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProviderName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("UserId")
                        .HasName("Idx_Logins_UserId");

                    b.ToTable("Logins");
                });

            modelBuilder.Entity("Rewind.One.AuthServer.Entities.AppUserClaims", b =>
                {
                    b.HasOne("Rewind.One.AuthServer.Entities.AppUser", null)
                        .WithMany("Claims")
                        .HasForeignKey("AppUserId");
                });

            modelBuilder.Entity("Rewind.One.AuthServer.Entities.AppUserLogin", b =>
                {
                    b.HasOne("Rewind.One.AuthServer.Entities.AppUser", null)
                        .WithMany("Logins")
                        .HasForeignKey("AppUserId");
                });
#pragma warning restore 612, 618
        }
    }
}