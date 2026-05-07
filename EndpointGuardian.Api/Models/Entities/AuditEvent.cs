namespace EndpointGuardian.Api.Models.Entities;

public class AuditEvent
{
    private AuditEvent()
    {
        Id = "";
        Actor = "";
        EntityId = "";
        Summary = "";
    }

    public string Id { get; private set; }
    public AuditActionType ActionType { get; private set; }
    public AuditEntityType EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string Actor { get; private set; }
    public DateTime TimestampUtc { get; private set; }
    public string Summary { get; private set; }

    public AuditEvent(
        string id,
        AuditActionType actionType,
        AuditEntityType entityType,
        string entityId,
        string actor,
        string summary)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Audit event id cannot be empty.");

        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("Entity id cannot be empty.");

        if (string.IsNullOrWhiteSpace(actor))
            throw new ArgumentException("Actor cannot be empty.");

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary cannot be empty.");

        Id = id;
        ActionType = actionType;
        EntityType = entityType;
        EntityId = entityId;
        Actor = actor;
        Summary = summary;
        TimestampUtc = DateTime.UtcNow;
    }
}