using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    public partial class EasyjobDb : DbContext
    {
        public EasyjobDb()
        {
        }

        public EasyjobDb(DbContextOptions<EasyjobDb> options)
            : base(options)
        {
        }

        public virtual DbSet<Job2Tour> Job2Tours { get; set; }
        public virtual DbSet<Stuff> Stuffs { get; set; }
        public virtual DbSet<Tour> Tours { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=NB-HK-01;Database=easyjob;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Job2Tour>(entity =>
            {
                entity.HasKey(e => e.IdTourJob)
                    .HasName("PK__job2Tour__C7FEF4AB2118FC42");

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.InOut).IsUnicode(false);

                entity.Property(e => e.Service).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.Property(e => e.Time).IsUnicode(false);
            });

            modelBuilder.Entity<Stuff>(entity =>
            {
                entity.HasKey(e => e.IdStuff)
                    .HasName("PK__Stuff__2B1267240E3F9C80");

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.EmployeeName1).IsUnicode(false);

                entity.Property(e => e.EmployeeName2).IsUnicode(false);

                entity.Property(e => e.EmployeeNr).IsUnicode(false);

                entity.Property(e => e.EmployeeType).IsUnicode(false);

                entity.Property(e => e.Employer).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(e => e.IdTour)
                    .HasName("PK__Tour__860C736FD11BF378");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).IsUnicode(false);

                entity.Property(e => e.Driver).IsUnicode(false);

                entity.Property(e => e.Master).IsUnicode(false);

                entity.Property(e => e.SecDriver).IsUnicode(false);

                entity.Property(e => e.TourDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TourName).IsUnicode(false);

                entity.Property(e => e.VehicleNr).IsUnicode(false);
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.IdVehicle)
                    .HasName("PK__Vehicle__64D74CC801A7CAE3");

                entity.Property(e => e.Comment).IsUnicode(false);

                entity.Property(e => e.Owner).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.Property(e => e.VehicleNr).IsUnicode(false);

                entity.Property(e => e.VehicleType).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
