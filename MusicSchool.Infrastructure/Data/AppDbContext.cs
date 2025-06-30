using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicSchool.Core.Entities;

namespace MusicSchool.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<School> Schools => Set<School>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<TeacherStudent> TeacherStudents => Set<TeacherStudent>();
        public DbSet<SchoolStudent> SchoolStudents => Set<SchoolStudent>();
        public DbSet<SchoolTeacher> SchoolTeachers=> Set<SchoolTeacher>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Reglas para unicidad
            modelBuilder.Entity<School>().HasIndex(s => s.Code).IsUnique();
            modelBuilder.Entity<Student>().HasIndex(s => s.IdentificationNumber).IsUnique();
            modelBuilder.Entity<Teacher>().HasIndex(t => t.IdentificationNumber).IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}
