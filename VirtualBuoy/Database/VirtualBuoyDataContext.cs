using Microsoft.EntityFrameworkCore;
using Models;
using Models.CourseItems;
using System.IO;
using Xamarin.Essentials;

namespace Database
{
    public class VirtualBuoyDataContext : DbContext
    {
        public DbSet<CourseMark> CourseMarks { get; set; }

        public DbSet<Settings> Settings { get; set; }



        public VirtualBuoyDataContext()
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "VirtualBuoy.db3");

            optionsBuilder
                .UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogItem>().HasKey(m => m.Id);
            modelBuilder.Entity<LogItem>().OwnsOne(m => m.Data);
            modelBuilder.Entity<LogItem>().HasOne(m => m.ParentLog).WithMany(m => m.LogEntries).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<LogItem>().HasOne(m => m.Mark);

            modelBuilder.Entity<CourseMark>().HasKey(m => m.Id);
            modelBuilder.Entity<CourseMark>().OwnsOne(p => p.Position);

            modelBuilder.Entity<Log>().HasKey(m => m.Id);
        }
    }
}