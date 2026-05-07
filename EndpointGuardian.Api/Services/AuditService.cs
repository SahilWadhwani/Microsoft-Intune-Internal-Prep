using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class AuditService : IAuditService
{
    private readonly IAuditEventRepository _auditRepository;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAuditEventRepository auditRepository,
        ILogger<AuditService> logger)
    {
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public async Task RecordAsync(
        AuditActionType actionType,
        AuditEntityType entityType,
        string entityId,
        string actor,
        string summary)
    {
        var auditEvent = new AuditEvent(
            Guid.NewGuid().ToString(),
            actionType,
            entityType,
            entityId,
            actor,
            summary);

        await _auditRepository.AddAsync(auditEvent);

        _logger.LogInformation(
            "Audit event {AuditEventId} recorded for {ActionType} on {EntityType}:{EntityId} by {Actor}",
            auditEvent.Id,
            actionType,
            entityType,
            entityId,
            actor);
    }

    public async Task<PagedAuditEventsResponse> GetAuditEventsAsync(GetAuditEventsQuery query)
    {
        var events = (await _auditRepository.GetAllAsync()).AsEnumerable();

        if (query.ActionType is not null)
            events = events.Where(e => e.ActionType == query.ActionType);

        if (query.EntityType is not null)
            events = events.Where(e => e.EntityType == query.EntityType);

        if (!string.IsNullOrWhiteSpace(query.EntityId))
            events = events.Where(e => e.EntityId == query.EntityId);

        if (!string.IsNullOrWhiteSpace(query.Actor))
            events = events.Where(e => e.Actor == query.Actor);

        var totalCount = events.Count();

        var items = events
            .OrderByDescending(e => e.TimestampUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedAuditEventsResponse(
            items,
            query.Page,
            query.PageSize,
            totalCount);
    }

    private static AuditEventResponse ToResponse(AuditEvent auditEvent)
    {
        return new AuditEventResponse(
            auditEvent.Id,
            auditEvent.ActionType,
            auditEvent.EntityType,
            auditEvent.EntityId,
            auditEvent.Actor,
            auditEvent.TimestampUtc,
            auditEvent.Summary);
    }
}