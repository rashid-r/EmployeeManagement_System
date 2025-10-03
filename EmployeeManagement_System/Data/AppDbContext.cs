using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection.Emit;

namespace EmployeeManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { } // Parameterless constructor for design time

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This connection string is used for design time (migrations)
                optionsBuilder.UseSqlServer("Server=TRYCATCHER;Database=EmployeeManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HireDate).IsRequired();
                entity.Property(e => e.AbsentDays).IsRequired();
                entity.Property(e => e.MonthlySalary)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.WorkingDaysPerMonth)
                    .IsRequired()
                    .HasDefaultValue(22);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed initial admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@company.com",
                    PasswordHash = BCryptSimulatedHasher.HashPassword("admin123")
                }
            );
        }
    }
}