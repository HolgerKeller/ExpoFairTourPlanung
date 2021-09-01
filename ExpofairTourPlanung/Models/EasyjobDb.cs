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
                    .HasName("PK__job2Tour__C7FEF4AB2226B6CE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
