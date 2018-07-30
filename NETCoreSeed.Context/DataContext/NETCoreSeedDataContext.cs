using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Context.DataContext
{
    public class NETCoreSeedDataContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // define the database to use
            //optionsBuilder.UseSqlServer(Runtime.ConnectionString);
            optionsBuilder.UseMySql(Runtime.MySqlConnectionString);

            //log all EF Core queries to console (useful for dev env)
            LoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new TraceLoggerProvider());
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Exclusion
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<ValidationError>();

            //Inclusion
            modelBuilder.AddConfiguration(new ActivitiesMap());
        }
    }
}
