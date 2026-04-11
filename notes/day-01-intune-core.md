
# Day 01: Thinking Like an Intune / Enterprise Backend Engineer

## 🎯 Learning Objectives
* **Product Clarity:** Define Intune beyond "an admin dashboard."
* **Pillar Mastery:** Understand how Compliance, Device Management, and Access Control intersect.
* **The Ecosystem:** Situate Microsoft Graph and Security Copilot within the architecture.
* **Project Vision:** Commit to the **Endpoint Guardian** project and its core entities.
* **Mindset Shift:** Move from "learning a framework" to "learning a product-service-security system."

---

## 🏗️ What is Intune, Really?
Microsoft defines it as a cloud-based endpoint management solution. For a backend engineer, it is a **Zero Trust Policy Engine**.

> **The Key Insight:** Intune sits in the path between devices, apps, and administrators. It manages who is trying to access resources, the state of their device, and whether that state satisfies a defined policy.

### The Core Pillars
| Pillar | Backend Mindset |
| :--- | :--- |
| **Device Management** | Modeling state transitions, handling stale telemetry, and resource lifecycle. |
| **App Management** | Assignment logic, compatibility rules, and deployment reporting. |
| **Compliance** | **The Core Logic:** Deterministic evaluation, pass/fail rules, and explainable failure reasons. |
| **Conditional Access** | Using Intune signals (compliant/non-compliant) to gate resource access. |

---

## 📊 The "Translation Layer" (Stack Mapping)
As a Python/Go developer, here is how you should mentally map your existing knowledge to the .NET world:

| Python/FastAPI Concept | ASP.NET Core Equivalent |
| :--- | :--- |
| `Depends()` / DI | Built-in Dependency Injection (DI) Container |
| Pydantic Models | DTOs / Records / Validation Patterns |
| Middleware | Middleware Pipeline |
| `appsettings.json` | Options Pattern / Configuration |
| `async def` | `async Task<IActionResult>` |
| Go System Explicitness | Interface-based Service Boundaries |

---

## 🛡️ Project Blueprint: Endpoint Guardian
**Goal:** A policy-driven device compliance service that simulates internal Intune-flavored backend workflows.

### Core Domain Entities
* `ManagedDevice`: Identity and posture data (OS, Encryption, etc.).
* `CompliancePolicy`: The rule sets devices must meet.
* `PolicyAssignment`: Mapping policies to specific groups.
* `ComplianceEvaluation`: The record of a pass/fail decision.
* `RemediationAction`: Instructions for an admin if a device fails.
* `AuditEvent`: A chronological log of state changes.

### Essential Engineering Behaviors
To stand out as a Microsoft intern, you must prioritize:
1.  **Observability:** Log for debuggability; use Correlation IDs.
2.  **Edge Case Reasoning:** What happens if a device hasn't checked in for 30 days?
3.  **Explainability:** Don't just return `False`; return *why* (e.g., "Encryption disabled").
4.  **Backward Compatibility:** Ensure API changes don't break existing automation.

---

## 📝 Checkpoints & Tasks

### [ ] Knowledge Check
* [ ] **Question:** Why is a compliance service not just a CRUD app?
    * *Answer:* Because it involves active evaluation logic, state transitions based on telemetry, and integration with access-control decisions.
* [ ] **Question:** What are 5 posture fields to track?
    * *Fields:* OS Version, Encryption Status, Password Set, Defender Active, Last Check-in.

### [ ] Build Task
1.  Initialize the repo structure:
    ```text
    /notes
    /endpoint-guardian
    ```
2.  Create a `project-design.md` inside `notes/` detailing the 6 entities and 5 core workflows.
3.  Draft 3 API endpoints:
    * `POST /devices` (Registration)
    * `POST /policies` (Management)
    * `POST /devices/{id}/evaluate` (Execution)

---

## 📚 Reference Documentation
* [Microsoft Intune Overview](https://learn.microsoft.com/en-us/mem/intune/fundamentals/what-is-intune)
* [Device Compliance Policies](https://learn.microsoft.com/en-us/mem/intune/protect/device-compliance-get-started)
* [Microsoft Graph for Intune](https://learn.microsoft.com/en-us/graph/api/resources/intune-graph-overview)

---
> **Day 1 Recap:** Stop thinking about code. Start thinking about **State, Policy, and Correctness.** Tomorrow, we write our first C# lines with this context in mind.