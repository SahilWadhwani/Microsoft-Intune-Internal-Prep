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

    public DbSet<RemediationAction> RemediationActions => Set<RemediationAction>();

    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureManagedDevice(modelBuilder);
        ConfigureCompliancePolicy(modelBuilder);
        ConfigurePolicyAssignment(modelBuilder);
        ConfigureRemediationAction(modelBuilder);
        ConfigureAuditEvent(modelBuilder);
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

    private static void ConfigureRemediationAction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RemediationAction>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.DeviceId)
                .IsRequired();

            entity.Property(a => a.ActionType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(a => a.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(a => a.RequestedBy)
                .IsRequired()
            .HasMaxLength(200);

            entity.Property(a => a.ResultMessage)
                .HasMaxLength(1000);
        });

    }

    private static void ConfigureAuditEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ActionType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.EntityType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.EntityId)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Actor)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Summary)
                .IsRequired()
                .HasMaxLength(1000);
        });
    }
  
}