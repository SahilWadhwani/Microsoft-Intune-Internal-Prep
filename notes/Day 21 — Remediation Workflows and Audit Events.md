# Day 21 — Remediation Workflows and Audit Events

Today, we add the **"Action & Memory"** layer to **Endpoint Guardian**. We’ve spent the last 20 days building a system that can *detect* problems; today, we build the system that **fixes** them and **remembers** who did it. 

This is heavily aligned with how **Microsoft Intune** works—where every remote action (like a device wipe or sync) is tracked in a permanent audit log.

---

## 🎯 Learning Objectives
* Understand **Remediation** as a multi-step workflow (Lifecycle), not just a checkbox.
* Enforce **State Transitions** (e.g., you can't "Complete" an action that hasn't "Started").
* Model **Audit Events** as first-class citizens in your database.
* Implement **Idempotency** (preventing duplicate active fix requests).
* Connect **Day 20 Identity** to **Day 21 Traceability** using the "Actor" claim.

---

## 🛠️ 1. Remediation: The Lifecycle of a Fix

In a professional system, we don't just say "Fix it." We track the fix from the moment an admin clicks the button until the device confirms it's done.

### The Status State Machine
We use an `enum` to track the "Health" of the fix:
* **Pending:** Requested, but the device hasn't checked in yet.
* **InProgress:** The device is currently attempting the fix.
* **Completed / Failed:** The final result.
* **Cancelled:** An admin stopped the fix before it finished.



---

## 📜 2. Audit Events: The System's Memory

There is a massive difference between **Application Logs** (ILogger) and **Audit Events**.

| Feature | ILogger (Diagnostic Logs) | Audit Events (Security Records) |
| :--- | :--- | :--- |
| **Target Audience** | Developers / DevOps | Security Admins / Auditors |
| **Storage** | Console, Splunk, or Text files | **Durable Database Table** |
| **Content** | "Request took 200ms" | "Admin-001 changed Policy-X" |
| **Lifespan** | Temporary (Days/Weeks) | Permanent (Months/Years) |



---

## 🏗️ 3. The "No Double-Work" Rule (Idempotency)

A major sign of a "Senior" backend engineer is thinking about **Race Conditions**.
* **Scenario:** An admin accidentally double-clicks the "Enable Encryption" button.
* **The Guard:** Your `RemediationService` should check the database first. If there is already a `Pending` or `InProgress` encryption task for that device, you return a **409 Conflict** instead of creating a second one.

---

## 🔒 4. Tying it all together with "The Actor"

Because of the work we did on **Day 20**, your `AuditService` can now record exactly **Who** performed an action.

When an admin calls:
`POST /api/devices/{id}/remediation-actions`

1.  We extract the `sub` (Subject) from the JWT token using `User.GetActorId()`.
2.  We save that string in the `RequestedBy` column of the `RemediationAction` table.
3.  We pass that same string to `_auditService.RecordAsync()`.

**Result:** You have a perfect paper trail: *"Sahil requested a 'Remote Wipe' on Laptop-07 at 12:00 PM."*

---

## 📝 5. Build Tasks for Today

### Step 1: The Models
* Create the `RemediationAction` entity with its `Start()`, `Complete()`, and `Fail()` logic.
* Create the `AuditEvent` entity to store the "Who, What, When."

### Step 2: The Data Layer
* Add both entities to your `EndpointGuardianDbContext`.
* **Migration Time:** Run `dotnet ef migrations add AddRemediationAndAudit` and `dotnet ef database update`.

### Step 3: The Logic (Services)
* Implement `RemediationService` (Enforce the lifecycle and prevent duplicates).
* Implement `AuditService` (The central hub for recording history).

### Step 4: The API (Controllers)
* Create `RemediationActionsController` protected by `[Authorize(Policy = "CanExecuteRemediation")]`.
* Create `AuditEventsController` protected by `[Authorize(Policy = "CanReadAudit")]`.

---

## 🏁 End-of-Session Recap
* **Traceability is everything** in security. If it isn't in the Audit Log, it didn't happen.
* **Domain logic belongs in the Entity.** Your `RemediationAction` should know it can't move from `Completed` back to `InProgress`.
* **Actor Identity is inherited.** We pull the "Who" from the token, ensuring the user can't lie about their identity in the logs.

*Our project is now a "System of Record," which is exactly what Microsoft looks for in their platform engineering teams.

---
**Ready to run your migration and trigger your first audited remediation action?**

*(Rule 2: Expert Guide applied)*
**If an admin deletes a device, should we also delete the Audit Events associated with that device? Why or why not?**