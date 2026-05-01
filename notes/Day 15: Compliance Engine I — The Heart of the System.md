# Day 15: Compliance Engine I — The Heart of the System

This is a massive milestone, Sahil. This is where **Endpoint Guardian** transitions from a collection of APIs into a living, breathing **policy-driven compliance system**.

Until now, you've built the ingredients (Devices, Policies, Assignments). Today, we build the **Chef**—the engine that combines them to make a definitive security decision.

---

## 🎯 Learning Objectives
* **Deterministic Evaluation:** Create a system that produces consistent, explainable results.
* **Structured Failures:** Move beyond simple strings to machine-readable error codes.
* **Effective Resolution:** Link the `PolicyResolver` to the evaluation flow.
* **Handling Missing Data:** Use C# nullables to distinguish between "Off" and "Never Reported."

---

## 💡 1. The Mindset Shift: Decisions, Not Booleans
A weak compliance engine returns `false`. A professional enterprise engine returns a **Decision Report**.

In the Microsoft/Intune world, a compliance result must be **explainable**. If a device is blocked from email (Conditional Access), the admin needs to know *exactly* which policy version failed and why.



---

## 🚦 2. The Compliance Status Model
We need more than just "Pass/Fail." We need to account for system errors and missing data.

| Status | Meaning |
| :--- | :--- |
| **Compliant** | Device satisfied all applicable active policies. |
| **NonCompliant** | At least one applicable policy failed. |
| **Unknown** | No active policies assigned or zero posture data available. |
| **Error** | A system failure occurred (e.g., malformed policy logic). |

### Missing vs. Failed Data
This is a critical distinction for security:
* **`false`**: The device reported that encryption is **Off**. (Action: Turn it on).
* **`null`**: The device **has not told us** if encryption is on or off. (Action: Fix the reporting agent).

> **Design Choice:** For today, if a required posture field is `null`, we treat it as **NonCompliant** with the code `ENCRYPTION_NOT_REPORTED`.



---

## 🏗️ 3. Structured Failure Reasons
We are upgrading from `List<string>` to a structured record. This allows dashboards to filter by error types (e.g., "Show me all devices with `OS_VERSION_TOO_LOW`").

```csharp
public record FailureReason(
    string Code,      // Machine-readable (e.g., "DEFENDER_DISABLED")
    string Message    // Human-readable (e.g., "Defender is required but is disabled.")
);
```

### Common Codes to Implement:
* `OS_VERSION_TOO_LOW`
* `ENCRYPTION_NOT_REPORTED` / `ENCRYPTION_DISABLED`
* `PASSWORD_MISSING`
* `CHECKIN_STALE`

---

## 🛠️ 4. Data Modeling (The Report Structure)
We want a hierarchical result. If a device has 3 policies assigned, the report should show the **Overall** status and then the **Per-Policy** breakdown.



```csharp
public record PolicyEvaluationResult(
    string PolicyId,
    string PolicyName,
    int PolicyVersion,
    ComplianceStatus Status,
    List<FailureReason> FailureReasons
);

public record DeviceComplianceEvaluationResult(
    string DeviceId,
    ComplianceStatus OverallStatus,
    List<PolicyEvaluationResult> PolicyResults,
    DateTime EvaluatedAtUtc
);
```

---

## ⚙️ 5. The Core Engine: `BasicComplianceEvaluator`
This service has one job: Compare one `ManagedDevice` against one `CompliancePolicy`.

```csharp
public PolicyEvaluationResult EvaluatePolicy(ManagedDevice device, CompliancePolicy policy)
{
    var failures = new List<FailureReason>();

    // OS Version Check
    if (device.OsVersion < policy.MinimumOsVersion)
    {
        failures.Add(new FailureReason("OS_VERSION_TOO_LOW", "..."));
    }

    // Encryption Check (Handling Nulls!)
    if (policy.RequireEncryption)
    {
        if (device.CurrentPostureSnapshot?.IsEncrypted is null)
            failures.Add(new FailureReason("ENCRYPTION_NOT_REPORTED", "..."));
        else if (device.CurrentPostureSnapshot.IsEncrypted == false)
            failures.Add(new FailureReason("ENCRYPTION_DISABLED", "..."));
    }

    // ... Check Password, Defender, and Freshness ...

    var status = failures.Count == 0 ? ComplianceStatus.Compliant : ComplianceStatus.NonCompliant;
    return new PolicyEvaluationResult(policy.Id, policy.Name, policy.Version, status, failures);
}
```

---

## 🌊 6. The Orchestration: `DeviceComplianceService`
This service pulls everything together:
1. **Find** the Device.
2. **Resolve** all applicable policies (via `IEffectivePolicyResolver`).
3. **Loop** through policies and call the Evaluator.
4. **Aggregate** the results (If one fails, the device is `NonCompliant`).
5. **Update** the `CurrentComplianceStatus` on the device entity.

---

## 🧪 7. Manual Test Flow (The Final Check)
1. **Create Device:** OS 12, Encryption False.
2. **Create Policy:** Requires OS 13, Encryption True.
3. **Assign Policy:** Assign to the "Windows" platform.
4. **Trigger Evaluation:** `POST /api/devices/{id}/evaluations`.
5. **Verify:** Check if the device in the list now shows `NonCompliant`.

---

## 🏁 End-of-Session Recap
* **Compliance is a decision pipeline**, not a simple check.
* **Structured logging** and **failure codes** make your system "admin-friendly."
* **Separation of concerns:** The `Resolver` finds the rules; the `Evaluator` checks the rules; the `Service` manages the outcome.

