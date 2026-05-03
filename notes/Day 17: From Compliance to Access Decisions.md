# Day 17: From Compliance to Access Decisions

Welcome to Day 17. Today, we move beyond the "Is this device healthy?" question and start answering the "Can this device touch our data?" question. This is the bridge where **Endpoint Guardian** transitions from a health monitor into a **Zero Trust** security platform.

---

## 🎯 Learning Objectives
* **Zero Trust Mindset:** Understand why compliance is only one "signal" in a broader security story.
* **Decision Engineering:** Distinguish between a *Compliance Result* and an *Access Decision*.
* **Mock Decision Engine:** Model structured outcomes like `Allow`, `Block`, and `RequireMfa`.
* **API Integration:** Build a mock Conditional Access endpoint that consumes your compliance logic.

---

## 💡 1. The Mindset Shift: Compliance ≠ Access
A device can be 100% compliant but still be blocked. Why? Because the **user's risk** might be high, or they might be logging in from an **unknown location**.

* **Compliance (Intune's Job):** Does the device satisfy our health rules?
* **Conditional Access (Entra's Job):** Given the context (User + Device + Risk + App), should we let them in?


---

## 🏗️ 2. The Decision Pipeline
In a modern security backend, we follow a specific flow:
**Signals** → **Policy Evaluation** → **Access Decision** → **Enforcement**

### The Signals we are modeling today:
* **User Identity:** Who is asking?
* **Device Identity:** What hardware are they using?
* **Compliance Status:** Did our Day 16 engine say they were healthy?
* **Resource:** What app are they trying to reach (e.g., "Microsoft365")?
* **Risk Level:** Is this a low, medium, or high-risk session?



---

## 📦 3. Data Models for Access
We need structured enums and records to represent these high-stakes decisions.

### Access Decision Types
```csharp
public enum AccessDecisionType
{
    Allow,               // Access permitted
    Block,               // Access denied
    RequireMfa,          // Need a second factor
    RequireRemediation,  // Device is NonCompliant; go fix it
    RequireFreshCheckIn, // Compliance is Unknown; phone home
    Unknown              // Insufficient evidence
}
```

### Risk Levels
```csharp
public enum AccessRiskLevel { Low, Medium, High }
```

---

## 🚦 4. The Decision Matrix (The "Laws")
To keep our system deterministic, we follow a strict priority order for our mock service:

| Condition | Decision | Reason Code |
| :--- | :--- | :--- |
| Device Not Found | **Error (404)** | `DEVICE_NOT_FOUND` |
| Compliance Status is `Error` | **Block** | `DEVICE_COMPLIANCE_ERROR` |
| Compliance Status is `Unknown`| **RequireFreshCheckIn** | `DEVICE_COMPLIANCE_UNKNOWN` |
| Compliance Status is `NonCompliant`| **RequireRemediation** | `DEVICE_NONCOMPLIANT` |
| Session Risk is `High` | **Block** | `HIGH_RISK_SESSION` |
| Risk is `Medium` + MFA Missing | **RequireMfa** | `MFA_REQUIRED` |
| All Requirements Met | **Allow** | `ACCESS_REQUIREMENTS_SATISFIED` |



---

## ⚙️ 5. Implementation Logic: `AccessDecisionService`
This service acts as the "Bouncer." It pulls the latest status from the `DeviceRepository` but does **not** mutate it. Access decisions should be a "Read-Only" evaluation of the current state.

```csharp
private static AccessDecisionType Decide(
    ManagedDevice device,
    CreateAccessDecisionRequest request,
    List<AccessDecisionReason> reasons)
{
    // 1. Check Compliance First
    if (device.CurrentComplianceStatus == ComplianceStatus.NonCompliant)
    {
        reasons.Add(new AccessDecisionReason("DEVICE_NONCOMPLIANT", "Remediation required."));
        return AccessDecisionType.RequireRemediation;
    }

    // 2. Check Session Risk
    if (request.RiskLevel == AccessRiskLevel.High)
    {
        reasons.Add(new AccessDecisionReason("HIGH_RISK_SESSION", "Risk too high."));
        return AccessDecisionType.Block;
    }

    // ... handle MFA and Success cases ...
    return AccessDecisionType.Allow;
}
```

---

## 🧪 6. Testing the Workflow
A "Product-Level" test for today involves the full end-to-end chain:
1. **Onboard** a device.
2. **Assign** a policy.
3. **Evaluate** compliance (ensure it's `Compliant`).
4. **Request Access** for "Microsoft365" with `RiskLevel = Medium` and `MfaSatisfied = false`.
5. **Expected Outcome:** The API should return `RequireMfa`.

---

## 🏁 End-of-Session Recap
* **Compliance is a signal**, access is the outcome.
* **Unknown compliance** is a security risk—never "Allow" by default.
* **Separation of Concerns:** Our `AccessDecisionService` consumes data from the `ComplianceEngine` but doesn't try to *be* the compliance engine.
* **Explainability:** Providing `AccessDecisionReason` codes makes your API "Admin-Friendly" and ready for future AI/Copilot summarization.

**The Bigger Picture:** You’ve now built a system that doesn't just "store data"—it **makes decisions**. This is exactly the kind of engineering mindset needed for Microsoft's Security and Identity teams.

---
