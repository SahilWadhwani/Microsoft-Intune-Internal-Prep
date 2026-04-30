# Day 14: Build Assignment Workflows

Today is a massive milestone. We are building the **Bridge** between our two pillars: **Devices** (what exists) and **Policies** (what we require). 

In the world of **Microsoft Intune**, policies don't just float in the cloud; they are "deployed" or "assigned." By creating a dedicated assignment layer, we move away from simple "CRUD" and into building a real **Orchestration System**.

---

## 🎯 Learning Objectives
By the end of today, you will:
* Model **Policy Assignments** as a first-class domain concept.
* Support various **Target Types**: Specific Device, Entire Platform, or All Devices.
* Implement **Duplicate Prevention** to keep the system state clean.
* Build the logic for **Effective Policy Resolution** (the "Who gets What" logic).

---

## 🏗️ 1. The Assignment Mental Model
We must separate these three concerns to keep the architecture "Intune-ready":

* **Policy:** What are the requirements? (e.g., "Must have Encryption").
* **Assignment:** Where do these requirements apply? (e.g., "All Windows Devices").
* **Evaluation:** Does the device satisfy the assigned rules?



> **The "Senior" Insight:** Beginners often just add a `PolicyId` to the `Device` table. Don't do that. That’s "Hard-Coding" a relationship. By using an **Assignment Entity**, you can eventually support complex logic like "Exclude the Finance Group" or "Apply only during business hours" without touching the Device code.

---

## 📋 2. Domain Design: Target Types
We need a way to define **Scope**. We’ll start with these three target types:

| Target Type | Description | Example |
| :--- | :--- | :--- |
| **Device** | Assigned to one specific device ID. | `Policy-X` → `Laptop-01` |
| **Platform** | Assigned to every device on an OS. | `Windows-Security-Baseline` → `Windows` |
| **AllDevices**| A global baseline for every endpoint. | `Minimum-Password-Policy` → `Global` |

---

## 🛠️ 3. The Code: `PolicyAssignment` Entity
This lives in `Models/Entities`. Notice how we protect the "Invariants"—like ensuring a `TargetId` exists if we aren't targeting "All Devices."

```csharp
namespace EndpointGuardian.Api.Models.Entities;

public class PolicyAssignment
{
    public string Id { get; private set; }
    public string PolicyId { get; private set; }
    public AssignmentTargetType TargetType { get; private set; }
    public string? TargetId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public string AssignedBy { get; private set; }
    public bool IsActive { get; private set; }

    public PolicyAssignment(string id, string policyId, AssignmentTargetType type, string? targetId, string by)
    {
        // Validation: The Bouncer
        if (type != AssignmentTargetType.AllDevices && string.IsNullOrWhiteSpace(targetId))
            throw new ArgumentException("TargetId is required for specific assignments.");

        Id = id;
        PolicyId = policyId;
        TargetType = type;
        TargetId = targetId;
        AssignedBy = by;
        AssignedAtUtc = DateTime.UtcNow;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
}
```

---

## 🚦 4. API Design: Nested Resources
We use **Nested Routing** because an assignment's existence depends entirely on the policy it belongs to.

* **POST** `/api/policies/{policyId}/assignments` — "Create an assignment for this policy."
* **GET** `/api/policies/{policyId}/assignments` — "See who this policy is assigned to."
* **GET** `/api/assignments/{id}` — "Lookup a specific assignment directly."



---

## 🔍 5. Effective Policy Resolution
This is the "Brain" of the assignment layer. When a device checks in, we need to ask: *"Which policies actually apply to this specific device right now?"*

```csharp
public List<CompliancePolicy> ResolvePoliciesForDevice(ManagedDevice device)
{
    var activeAssignments = _assignmentRepository.GetAll()
        .Where(a => a.IsActive && AppliesToDevice(a, device));

    return activeAssignments
        .Select(a => _policyRepository.GetById(a.PolicyId))
        .Where(p => p is not null && p.IsActive)
        .ToList()!;
}
```



**Our "Conflict" Strategy:** If multiple policies apply (e.g., an "All Devices" policy AND a "Windows" policy), the device must satisfy **all** of them to be compliant. This is a "Security-First" approach.

---

## 🛡️ 6. Edge Cases to Handle
1.  **Policy Inactive:** If a policy is deactivated, the assignments should be ignored by the Resolver.
2.  **Duplicate Assignments:** Your Service should block creating a new assignment if an identical active one already exists. (Return a **409 Conflict**).
3.  **Invalid Target:** Don't allow assigning a "Windows Policy" to a "macOS" target ID.

---

## 📝 7. Build Tasks for Today
1.  **Models:** Create the `PolicyAssignment` entity and the `AssignmentTargetType` enum.
2.  **DTOs:** Build the `CreatePolicyAssignmentRequest` and response DTOs.
3.  **Repository:** Build the `IPolicyAssignmentRepository`.
4.  **Service:** Implement the `PolicyAssignmentService` with duplicate check logic.
5.  **Resolver:** Create the `EffectivePolicyResolver` service.
6.  **Controller:** Build the `PolicyAssignmentsController` with the nested `{policyId}` route.

---

## 🏁 End-of-Session Recap
* **Policy** = The Law. **Assignment** = The Scope.
* **Nested Routes** make the API relationship clear.
* **Target Validation** ensures Windows policies don't get assigned to Macs.
* **Effective Resolution** is the bridge that tells the Evaluator which rules to use.

**Next Up:** **Day 15 — The Compliance Engine.** This is where the magic happens—combining Device Posture + Effective Policies to produce a Compliance Decision.
