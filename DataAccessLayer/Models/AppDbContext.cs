using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role", "user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("user_pkey");

            entity.ToTable("user", "user");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.BirthDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Role).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.RoleId)
                .HasConstraintName("user_RoleId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
