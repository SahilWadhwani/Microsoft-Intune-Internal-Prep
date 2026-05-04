using EndpointGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EndpointGuardian.Api.Data;

public class EndpointGuardianDbContext : DbContext
{
    public EndpointGuardianDbContext(
        DbContextOptions<EndpointGuardianDbContext> options)
        : base(options)
    {
    }

    public DbSet<ManagedDevice> Devices => Set<ManagedDevice>();
    public DbSet<CompliancePolicy> Policies => Set<CompliancePolicy>();
    public DbSet<PolicyAssignment> PolicyAssignments => Set<PolicyAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureManagedDevice(modelBuilder);
        ConfigureCompliancePolicy(modelBuilder);
        ConfigurePolicyAssignment(modelBuilder);
    }

    private static void ConfigureManagedDevice(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ManagedDevice>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.DeviceName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(d => d.Platform)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(d => d.CurrentComplianceStatus)
                .HasConversion<string?>();

            entity.OwnsOne(d => d.CurrentPostureSnapshot, posture =>
            {
                posture.Property(p => p.IsEncrypted);
                posture.Property(p => p.HasPassword);
                posture.Property(p => p.DefenderEnabled);
                posture.Property(p => p.CapturedAtUtc);
            });
        });
    }

    private static void ConfigureCompliancePolicy(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompliancePolicy>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
        });
    }

    private static void ConfigurePolicyAssignment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PolicyAssignment>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.PolicyId)
                .IsRequired();

            entity.Property(a => a.TargetType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(a => a.TargetId)
                .HasMaxLength(200);

            entity.Property(a => a.AssignedBy)
                .IsRequired()
                .HasMaxLength(200);
        });
    }
}