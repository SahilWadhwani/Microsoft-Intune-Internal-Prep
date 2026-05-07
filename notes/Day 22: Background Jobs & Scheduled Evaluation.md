
# Day 22: Background Jobs & Scheduled Evaluation

Today, **Endpoint Guardian** becomes a "Living System." In a real-world platform like **Microsoft Intune**, compliance isn't just checked once; it's a continuous heartbeat. If a device was compliant at 9:00 AM but disabled its firewall at 10:00 AM, a background job is what catches that drift.

---

## 🏗️ 1. What is a Background Worker?

In ASP.NET Core, we use **Hosted Services** (specifically the `BackgroundService` class) to run code that isn't tied to an HTTP request. 

* **The Worker:** The "Alarm Clock." It handles the timing, the loop, and making sure the app stays alive.
* **The Job:** The "Workhorse." It contains the actual business logic of loading devices and calling the evaluation engine.



---

## 🛑 2. The "Scoped Service" Trap (Crucial!)

This is the #1 mistake developers make with background jobs. 
* **The Problem:** Your `DbContext` and `Repositories` are **Scoped** (they live for one request). But your **Background Worker** is a **Singleton** (it lives forever). You cannot "inject" a short-lived service into a long-lived one directly.
* **The Solution:** You inject the `IServiceScopeFactory`. Every time the "alarm" goes off, the worker creates a tiny, temporary **Scope** (a fake request), finishes the job, and then throws that scope away.



---

## 🎯 3. The Evaluation Logic: Deciding Who to Check

To keep the system efficient, we don't evaluate every single device every 30 seconds. We use a "Priority Filter":

1.  **New Devices:** If `LastComplianceEvaluationAtUtc` is null.
2.  **Unknowns:** If the status is `Unknown`.
3.  **Time-based Drift:** If the `ReevaluateAfterMinutes` threshold has passed (e.g., it’s been more than 60 minutes since we last looked at this device).



---

## 🛠️ 4. Configuration: The "ScheduledCompliance" Options

We never hardcode intervals. We use the **Options Pattern** so an admin can change how fast the heartbeat is without recompiling the code.

```json
"ScheduledCompliance": {
  "Enabled": true,
  "IntervalSeconds": 60,
  "MaxDevicesPerRun": 25,
  "ReevaluateAfterMinutes": 60
}
```

---

## 📝 5. Build Tasks for Today

1.  **Update Model:** Add `LastComplianceEvaluationAtUtc` to the `ManagedDevice` class and run a migration.
2.  **Create the Job:** Build `ScheduledComplianceEvaluationJob` to handle the device-filtering logic.
3.  **Create the Worker:** Build the `BackgroundService` that creates the DI scope and runs the loop.
4.  **Registration:** Register the worker in `Program.cs` using `builder.Services.AddHostedService<T>()`.
5.  **Audit Integration:** Ensure that if a scheduled check finds a **Non-Compliant** device, it automatically writes an **Audit Event** as "System:Scheduled-Worker."

---

## 🏁 End-of-Session Recap
* **Continuity:** Compliance is a state, not a one-time event.
* **Efficiency:** We use paging (`MaxDevicesPerRun`) and filtering to avoid overwhelming the database.
* **Safety:** We use `CancellationToken` to ensure that if the server shuts down, the background job stops gracefully instead of corrupting data.

**You're building "Operational Maturity" now, Sahil.** This is exactly the kind of "System Reliability" work that Microsoft Intune teams do at a massive scale.

---
**Ready to start your app and watch the console logs as the background worker "wakes up" for the first time?**

*(Rule 2: Expert Guide applied)*
**If the background job finds 10,000 devices that need checking, but your `MaxDevicesPerRun` is set to 25, how many cycles will it take to finish the work?**