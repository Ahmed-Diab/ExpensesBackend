using Expenses.Core.Moduels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expenses.EF
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Expense> Expenses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Expense>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Category>()
               .HasOne(c => c.User)
               .WithMany()
               .HasForeignKey(u => u.UserId)
               .OnDelete(DeleteBehavior.ClientSetNull);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateOnly>()
               .HaveConversion<DateOnlyConverter>()
               .HaveColumnType("date");

            builder.Properties<TimeOnly>()
               .HaveConversion<TimeOnlyConverter>()
               .HaveColumnType("time");
        }
    }
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d))
        { }
    }

    public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
    {
        public TimeOnlyConverter() : base(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan))
        {
        }
    }
}
