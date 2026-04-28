

# Day 11: Domain Modeling for Endpoint Guardian

## 🎯 Learning Objectives
* **Identity vs. Value:** Distinguish between **Entities** and **Value Objects**.
* **Aggregate Roots:** Identify the "boss" objects that control consistency.
* **Invariants:** Define the unbreakable rules of your business logic.
* **State Management:** Model transitions (e.g., Pending → In-Progress → Completed).

---

## 🏗️ 1. Entities vs. Value Objects
In C#, not every class is created equal. Understanding this distinction is the first step toward a clean architecture.

### Entities (Identity Matters)
An **Entity** is something that has a unique ID and a lifecycle. Even if every other property changes (like a device's name), it is still the *same* object because its ID stays the same.
* **Examples:** `ManagedDevice`, `CompliancePolicy`, `RemediationAction`.

### Value Objects (Values Matter)
A **Value Object** has no identity. It is defined entirely by the data it holds. If you change a value, it’s a different object. They should be **Immutable** (read-only).
* **Examples:** `DevicePostureSnapshot`, `FailureReason`, `OsVersion`.



---

## 👑 2. Aggregate Roots: The Consistency Boundary
In a complex system, objects are rarely alone. An **Aggregate** is a cluster of associated objects that we treat as a single unit for data changes. The **Aggregate Root** is the "front door" to that cluster.

* **The Root:** `ManagedDevice`
* **The "Children":** `PostureHistory`, `EvaluationResults`.
* **The Rule:** You should never reach inside and change a `PostureHistory` record directly. You ask the `ManagedDevice` to `UpdatePosture()`, and the device handles the internal list itself. This ensures that the device can validate the data before saving it.



---

## 🛡️ 3. Invariants: The "Always True" Rules
An **Invariant** is a business rule that must **always** be satisfied. If an operation would break an invariant, the system must throw an error.

**Examples for Endpoint Guardian:**
* A `ManagedDevice` must always have a `Platform` assigned.
* A `ComplianceEvaluation` cannot be "Compliant" if it has one or more `FailureReasons`.
* A `RemediationAction` cannot be moved to "Completed" unless it was previously "In-Progress."

By enforcing these rules *inside* the class (using a constructor or methods), you prevent "impossible" states from ever reaching your database.

---

## 🔄 4. State Transitions & Lifecycle
Enterprise systems are rarely static. Things move through workflows. Instead of just changing a `Status` string, we model the **Transitions**.

**Remediation Lifecycle:**
1.  **Pending:** Action created, waiting for the device to sync.
2.  **In-Progress:** Device has acknowledged the command.
3.  **Completed:** Device reports the fix is successful.
4.  **Failed:** Something went wrong (e.g., timeout or permission error).



---

## 🛠️ Build Task for Day 11: The Domain Refactor

Today’s task is to move your logic out of the Services and into the **Entities**.

1.  **Create Value Objects:** Turn your "Failure Reasons" into a `record` called `FailureReason` with a `Code` and a `Message`.
2.  **Enforce Invariants:** Add a constructor to `ManagedDevice` that requires an `Id` and `Name`. If they are null, throw an `ArgumentException`.
3.  **Model Transitions:** Create a method in `ManagedDevice` called `ProcessCheckIn(DevicePosture posture)`. This method should:
    * Update the `LastCheckInUtc`.
    * Add the new posture to a private history list.
    * Check if the device was previously "Stale" and flip the flag.
4.  **Audit Events:** Create an `AuditEvent` entity. Every time a major state change happens (Registration, Evaluation, Remediation), your service should generate one of these.

---
> **Day 11 Recap:** You aren't just storing data anymore; you're simulating a living ecosystem. By protecting your **Invariants** and respecting **Aggregate Roots**, you ensure that **Endpoint Guardian** stays reliable as it grows.
