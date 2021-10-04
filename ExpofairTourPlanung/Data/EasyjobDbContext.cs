using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ExpofairTourPlanung.Models;

namespace ExpofairTourPlanung.Data
{
   
    public class EasyjobDbContext : DbContext
    {
        public EasyjobDbContext()
        {
        }

        public EasyjobDbContext(DbContextOptions<EasyjobDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Job2Tour> Job2Tours { get; set; }
        public virtual DbSet<Stuff> Stuffs { get; set; }
        public virtual DbSet<Tour> Tours { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<ExpoEvent> ExpoEvents { get; set; }
        public virtual DbSet<Stock2jobSP> Stock2JobSPs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Job2Tour>(entity =>
            {
                entity.HasKey(e => e.IdTourJob)
                    .HasName("PK__job2Tour__C7FEF4ABE2DEF2D7");
            });

            modelBuilder.Entity<Stuff>(entity =>
            {
                entity.HasKey(e => e.IdStuff)
                    .HasName("PK__Stuff__2B1267242F26DE37");
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(e => e.IdTour)
                    .HasName("PK__Tour__860C736F8718B294");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).IsUnicode(false);

                entity.Property(e => e.TourDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.IdVehicle)
                    .HasName("PK__Vehicle__64D74CC8C159C767");
            });

        }
    }

}
