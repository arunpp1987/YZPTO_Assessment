using Microsoft.EntityFrameworkCore;
using YPTO.DAL.Entities;

namespace YPTO.DAL.Data
{
    public class YptoDBContext : DbContext
    {
        public YptoDBContext(DbContextOptions<YptoDBContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationships and constraints here if needed
            modelBuilder.Entity<Training>()
                .HasOne(t => t.Course)
                .WithMany(c => c.Trainings)
                .HasForeignKey(t => t.CourseId);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Training)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(s => s.TrainingId);
        }

    }
}