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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=NB-HK-01;Database=easyjob;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Job2Tour>(entity =>
            {
                entity.HasKey(e => e.IdTourJob)
                    .HasName("PK__job2Tour__C7FEF4AB51A70FA8");

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.InOut).IsUnicode(false);

                entity.Property(e => e.Service).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.Property(e => e.Time).IsUnicode(false);
            });

            modelBuilder.Entity<Stuff>(entity =>
            {
                entity.HasKey(e => e.IdStuff)
                    .HasName("PK__Stuff__2B126724F91D7B48");

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.EmployeeName1).IsUnicode(false);

                entity.Property(e => e.EmployeeName2).IsUnicode(false);

                entity.Property(e => e.EmployeeNr).IsUnicode(false);

                entity.Property(e => e.EmployeeType).IsUnicode(false);
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(e => e.IdTour)
                    .HasName("PK__Tour__860C736FD0CBD065");

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
                    .HasName("PK__Vehicle__64D74CC890905727");

                entity.Property(e => e.Comment).IsUnicode(false);

                entity.Property(e => e.VehicleNr).IsUnicode(false);

                entity.Property(e => e.VehicleType).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
