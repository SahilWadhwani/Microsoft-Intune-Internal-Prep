---

# Day 13: Build Policy Management APIs

Today, we add the second major pillar of **Endpoint Guardian**: Policy Management. This moves the project from a simple device list to a functional management system. 

In the Microsoft ecosystem, **Intune compliance policies** are sets of rules used to evaluate devices. Non-compliant devices can be blocked via Conditional Access—this is the logic we are building today.

---

## 🎯 Learning Objectives
By the end of today, you will:
* Understand the role of **Compliance Policies** in endpoint management.
* Design and implement resource-oriented **REST APIs** for policies.
* Model **Domain Entities** with built-in validation and versioning logic.
* Decouple **Policy Definition** from **Policy Assignment**.

---

## 💡 1. The Mindset Shift: Policy as a Resource
A policy is not just a configuration file; it is a reusable rule set with its own identity and lifecycle.

| Resource Type | Focus | Key Characteristics |
| :--- | :--- | :--- |
| **Device** | Observed Managed Endpoint | Changes posture, checks in, gets evaluated. |
| **Policy** | Admin-Defined Rules | Defines requirements, versioned, assigned to scopes. |



---

## 🏛️ 2. Domain Model: `CompliancePolicy`
We use a "Rich Domain Model" to ensure that a policy is always in a valid state.

```csharp
namespace EndpointGuardian.Api.Models.Entities;

public class CompliancePolicy
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int Version { get; private set; }
    public int MinimumOsVersion { get; private set; }
    public bool RequireEncryption { get; private set; }
    public bool RequirePassword { get; private set; }
    public bool RequireDefender { get; private set; }
    public int MaxCheckInAgeHours { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public CompliancePolicy(string id, string name, int minOs, bool enc, bool pass, bool def, int maxAge)
    {
        // Invariants: Business rules that must always be true
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.");
        if (minOs <= 0) throw new ArgumentException("Min OS must be > 0.");
        if (maxAge <= 0) throw new ArgumentException("Max age must be > 0.");

        Id = id;
        Name = name;
        MinimumOsVersion = minOs;
        RequireEncryption = enc;
        RequirePassword = pass;
        RequireDefender = def;
        MaxCheckInAgeHours = maxAge;
        Version = 1;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Reactivate() => IsActive = true;
}
```

---

## 📦 3. Data Transfer Objects (DTOs)
We separate the internal entity from the external API contract using **Requests**, **Responses**, and **Queries**.

### Create Request
```csharp
public record CreatePolicyRequest(
    string Name,
    int MinimumOsVersion,
    bool RequireEncryption,
    bool RequirePassword,
    bool RequireDefender,
    int MaxCheckInAgeHours
);
```

### Response Shapes
* **`PolicyResponse`**: Full detail for `GET /api/policies/{id}`.
* **`PolicySummaryResponse`**: Lightweight view for `GET /api/policies`.
* **`PagedPoliciesResponse`**: Wrapper for list results with metadata.

---

## 🚦 4. API Surface & Status Codes
We follow standard REST conventions to make the API predictable and automation-friendly.

| Method | Endpoint | Success Code | Failure Codes |
| :--- | :--- | :--- | :--- |
| **POST** | `/api/policies` | 201 Created | 400 Bad Request |
| **GET** | `/api/policies` | 200 OK | 400 (Invalid Paging) |
| **GET** | `/api/policies/{id}` | 200 OK | 404 Not Found |

---

## 🛠️ 5. Implementation Logic

### Policy Service
The service orchestrates the creation, validation, and mapping of policies.

```csharp
public PolicyResponse? CreatePolicy(CreatePolicyRequest request)
{
    // Validation Logic
    if (string.IsNullOrWhiteSpace(request.Name) || request.MinimumOsVersion <= 0) return null;

    var policy = new CompliancePolicy(
        Guid.NewGuid().ToString(),
        request.Name,
        request.MinimumOsVersion,
        request.RequireEncryption,
        request.RequirePassword,
        request.RequireDefender,
        request.MaxCheckInAgeHours
    );

    _policyRepository.Add(policy);
    _logger.LogInformation("Policy {PolicyId} created v{Version}", policy.Id, policy.Version);

    return ToPolicyResponse(policy);
}
```

### Dependency Injection
Register the new layers in `Program.cs` to enable the "Plumbing."

```csharp
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddSingleton<IPolicyRepository, InMemoryPolicyRepository>();
```

---

## 🧪 6. Manual Testing (cURL)

**Create a Policy:**
```bash
curl -X POST http://localhost:5000/api/policies \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Corporate Baseline",
    "minimumOsVersion": 13,
    "requireEncryption": true,
    "requirePassword": true,
    "requireDefender": true,
    "maxCheckInAgeHours": 72
  }'
```

**List Active Policies:**
```bash
curl "http://localhost:5000/api/policies?isActive=true&page=1&pageSize=10"
```

---

## 🏁 End-of-Session Recap
* **Separation of Concerns:** Policies and Devices live in different tables/repositories.
* **Versioning:** Essential for historical compliance audits.
* **Lifecycle:** Inactive policies are better than outright deletion for audit trails.
* **Architecture:** Service/Repository layering keeps the system maintainable as complexity grows.

**The Bigger Picture:**
1. **Device:** The observed endpoint.
2. **Policy:** The organization's rules.
3. **Evaluation:** (Coming soon) Where the two meet.

---
**Ready for Day 14 — Build Assignment Workflows?**