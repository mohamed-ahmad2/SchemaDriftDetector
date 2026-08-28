using Microsoft.EntityFrameworkCore;
using SchemaDriftDetector.Storage.Entities;

namespace SchemaDriftDetector.Storage.Context
{
    public class SchemaDriftDetectorDbContext : DbContext
    {
        public SchemaDriftDetectorDbContext(DbContextOptions<SchemaDriftDetectorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Endpoint> Endpoints => Set<Endpoint>();
        public DbSet<SchemaBaseline> SchemaBaselines => Set<SchemaBaseline>();
        public DbSet<SchemaVersion> SchemaVersions => Set<SchemaVersion>();
        public DbSet<PendingDrift> PendingDrifts => Set<PendingDrift>();
        public DbSet<DriftAlert> DriftAlerts => Set<DriftAlert>();
        public DbSet<Deploy> Deploys => Set<Deploy>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------------------------------------------
            // Endpoints
            // ------------------------------------------------------------
            modelBuilder.Entity<Endpoint>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.RouteTemplate, e.Environment })
                      .IsUnique();
            });

            // ------------------------------------------------------------
            // SchemaBaselines 
            // ------------------------------------------------------------
            modelBuilder.Entity<SchemaBaseline>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.HasOne(b => b.Endpoint)
                      .WithOne(e => e.SchemaBaseline)
                      .HasForeignKey<SchemaBaseline>(b => b.EndpointId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(b => b.EndpointId)
                      .IsUnique();
            });

            // ------------------------------------------------------------
            // SchemaVersions 
            // ------------------------------------------------------------
            modelBuilder.Entity<SchemaVersion>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.HasOne(v => v.Endpoint)
                      .WithMany(e => e.SchemaVersions)
                      .HasForeignKey(v => v.EndpointId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(v => v.Deploy)
                      .WithMany(d => d.SchemaVersions)
                      .HasForeignKey(v => v.DeployId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ------------------------------------------------------------
            // PendingDrifts 
            // ------------------------------------------------------------
            modelBuilder.Entity<PendingDrift>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.Endpoint)
                      .WithMany(e => e.PendingDrifts)
                      .HasForeignKey(p => p.EndpointId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.ChangeType)
                      .HasConversion<string>();

                entity.Property(p => p.Status)
                      .HasConversion<string>();
            });

            // ------------------------------------------------------------
            // DriftAlerts
            // ------------------------------------------------------------
            modelBuilder.Entity<DriftAlert>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Endpoint)
                      .WithMany(e => e.DriftAlerts)
                      .HasForeignKey(a => a.EndpointId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Deploy)
                      .WithMany(d => d.DriftAlerts)
                      .HasForeignKey(a => a.DeployId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(a => a.Severity)
                      .HasConversion<string>();
            });

            // ------------------------------------------------------------
            // Deploys
            // ------------------------------------------------------------
            modelBuilder.Entity<Deploy>(entity =>
            {
                entity.HasKey(d => d.Id);
            });
        }
    }
}