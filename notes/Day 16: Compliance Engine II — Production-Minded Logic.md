# Day 16: Compliance Engine II — Production-Minded Logic

Yesterday, we built the "happy path" engine. Today, we make it **serious**. In a real enterprise system like **Microsoft Intune**, the most important work isn't just checking a "True/False" flag—it’s handling the messy reality of missing data, stale check-ins, and conflicting policies.

---

## 🎯 Learning Objectives
* Define precise semantics for **Compliant**, **NonCompliant**, **Unknown**, and **Error**.
* Handle **Missing Telemetry** and **Stale Check-ins** with a security-first mindset.
* Implement **Deterministic Combination Logic** for multiple applicable policies.
* Bridge the gap between **Domain Outcomes** and **API Status Codes**.

---

## 💡 1. Defining the Semantics
Edge cases are the core of a compliance system. We need exact meanings for every possible outcome to ensure the system is predictable for admins.

| Status | Meaning | Scenario Example |
| :--- | :--- | :--- |
| **Compliant** | Satisfied all active requirements. | All rules pass. |
| **NonCompliant** | Failed at least one active requirement. | Encryption is disabled. |
| **Unknown** | Insufficient evidence to make a decision. | No policies assigned or no check-in data. |
| **Error** | The system failed to process the request. | Exception thrown during evaluation. |

---

## ⚖️ 2. The Combination Rule: Priority Order
When a device has three different policies assigned, we need a "Result Combiner" that follows a deterministic priority order.

### **The Priority Order:**
`Error` > `NonCompliant` > `Unknown` > `Compliant`

* **Error** dominates everything; if the system is broken, we cannot trust any other result.
* **NonCompliant** dominates unknown/compliant; if we found a hole, the device is unsafe.
* **Unknown** dominates compliance; we can't say a device is "safe" if some rules couldn't be checked.
* **Compliant** is only the result if every single policy passed perfectly.



---

## 🛠️ 3. Handling Missing Telemetry
In a production-minded engine, `null` is different from `false`.

* **Strategy:** If the **entire snapshot** is missing, the status is `Unknown`.
* **Strategy:** If a **required field** inside a snapshot is `null`, the status is `NonCompliant`.

This distinguishes between "The device hasn't phoned home yet" and "The device phoned home but refused to report its encryption status."

---

## 🏗️ 4. Upgraded Data Models
We are adding **Severity** and **Evaluation IDs** to make the results audit-ready.

```csharp
public enum FailureSeverity { Info, Warning, High, Critical }

public record FailureReason(
    string Code,
    string Message,
    FailureSeverity Severity
);

public record DeviceComplianceEvaluationResult(
    string EvaluationId, // Now tracking unique evaluation events
    string DeviceId,
    ComplianceStatus OverallStatus,
    List<PolicyEvaluationResult> PolicyResults,
    DateTime EvaluatedAtUtc
);
```

---

## ⚙️ 5. Implementation: The Result Combiner
This helper method acts as the "Jury" for the final device verdict. It belongs in your `DeviceComplianceService`.

```csharp
private static ComplianceStatus CombinePolicyResults(List<PolicyEvaluationResult> results)
{
    if (results.Count == 0) return ComplianceStatus.Unknown;

    if (results.Any(r => r.Status == ComplianceStatus.Error)) return ComplianceStatus.Error;
    if (results.Any(r => r.Status == ComplianceStatus.NonCompliant)) return ComplianceStatus.NonCompliant;
    if (results.Any(r => r.Status == ComplianceStatus.Unknown)) return ComplianceStatus.Unknown;

    return ComplianceStatus.Compliant;
}
```

---

## 🚦 6. API Semantics Table
Mapping domain outcomes to the correct HTTP behavior is key for a professional API.

| Situation | Domain Status | HTTP Status |
| :--- | :--- | :--- |
| Device ID not found | None | **404 Not Found** |
| Device exists, no policy assigned | `Unknown` | **200 OK** |
| Posture snapshot missing | `Unknown` | **200 OK** |
| Required field is `false` or `null` | `NonCompliant` | **200 OK** |
| Stale check-in (threshold exceeded) | `NonCompliant` | **200 OK** |
| Duplicate assignment creation | None | **409 Conflict** |

---

## 🧪 7. Manual Testing Checkpoints
To verify your Day 16 work, try these scenarios:
1. **The "Fresh" Device:** Create a device but do not call `CheckIn`. Evaluate it. (Expected: `Unknown` / `POSTURE_NOT_REPORTED`).
2. **The "Lazy" Device:** Call `CheckIn` with `null` for Defender status. Evaluate. (Expected: `NonCompliant` / `DEFENDER_NOT_REPORTED`).
3. **The "Stale" Device:** Manually update a device's `LastCheckInUtc` to 5 days ago. Evaluate against a 72-hour policy. (Expected: `NonCompliant` / `CHECKIN_STALE`).

---

## 🏁 End-of-Session Recap
* **Edge cases** define the quality of a compliance engine.
* **Unknown** is not a failure; it’s a request for more evidence.
* **NonCompliance** is not an error; it’s a valid security decision.
* **Structured reasons** make the system explainable to admins.

**The Bigger Picture:** Your compliance engine is now a "Semantics Engine." You are no longer just comparing numbers; you are defining the security posture of an organization.

**Next Up:** **Day 17 — Conditional Access.** We’ll bridge the gap between "Compliance Results" and "Access Decisions" (Allow/Block).

