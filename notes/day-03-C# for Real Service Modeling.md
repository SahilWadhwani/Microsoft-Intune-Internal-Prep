# Day 03: C# for Real Service Modeling

## 🎯 Learning Objectives
* **Defensive Modeling:** Using constructors and access modifiers to enforce valid states.
* **Contracts:** Implementing **Interfaces** to decouple logic from implementation.
* **Data Flow:** Using **Records** for snapshots and DTOs (Data Transfer Objects).
* **State Management:** Replacing fragile strings with Type-safe **Enums**.
* **Error Resilience:** Handling **Exceptions** like a backend engineer.

---

## 🏗️ Deep Dive: The Service Design Mindset

Yesterday was syntax; today is **Design**. At Microsoft-scale, "clever" code is a liability. "Boring, predictable, and explicit" code is the goal.

### 1. Constructors & Validation
Don't let invalid objects exist. If a `ManagedDevice` requires an `Id`, force it in the constructor.



```csharp
public ManagedDevice(string id, string deviceName)
{
    // Guard Clauses: Fail fast if data is bad
    if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id is required.");
    Id = id;
    DeviceName = deviceName;
}
```

### 2. Access Modifiers: The "Need to Know" Basis
The more you hide (`private`), the less you can break.



* **`public`**: The API surface.
* **`private set`**: The gold standard for domain models. Everyone can see the status, but only the object can change it.

### 3. Encapsulation
Move from "Data Bags" to "Behavioral Objects." Instead of manually changing properties, use methods like `CheckIn()` or `UpdatePosture()`. This ensures that whenever a property changes, the `LastCheckInUtc` is also updated automatically.



---

## 📜 Abstraction & Interfaces
This is the most critical concept for Day 3. An **Interface** (`IComplianceEvaluator`) is a contract. 

> **Why?** It allows you to swap logic without changing the caller. Today you use `BasicComplianceEvaluator`. Tomorrow, you might swap it for a `SecurityCopilotEvaluator`. The `Program` class won't even notice the difference because both follow the same "contract."



---

## 📊 Modeling Choices: Classes vs. Records vs. Enums

| Tool | Use Case | Example |
| :--- | :--- | :--- |
| **Class** | Long-lived entities with logic and state changes. | `ManagedDevice`, `CompliancePolicy` |
| **Record** | Immutable snapshots, API responses, or results. | `ComplianceEvaluationResult` |
| **Enum** | A fixed set of choices. Avoids "stringly-typed" bugs. | `ComplianceStatus`, `DevicePlatform` |

---

## ⚠️ Exceptional Thinking
In a policy engine, **Non-compliance is not an error**—it is a valid business outcome.
* **Do NOT throw** an exception because a device is unencrypted.
* **DO throw** an exception if the `CompliancePolicy` is `null` (the evaluator cannot function without it).



---

## 🛠️ Build Task: The Professional Evaluator

You are evolving the `Endpoint Guardian` project to include:
1.  **Enums** for Platforms and Status.
2.  **Interface** `IComplianceEvaluator` to define the evaluation contract.
3.  **Record** `ComplianceEvaluationResult` to return structured pass/fail data.
4.  **Guard Clauses** in constructors to prevent empty IDs or names.

### Sample Execution
```csharp
IComplianceEvaluator evaluator = new BasicComplianceEvaluator();
var result = evaluator.Evaluate(myDevice, myPolicy);

Console.WriteLine($"Status: {result.Status}"); // "NonCompliant"
```

---

## 📚 Reference Documentation
* [Interfaces in C#](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces)
* [Records (C# Reference)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)
* [Object-Oriented Programming (Encapsulation)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop)

---
> **Day 3 Recap:** You are no longer just writing code; you are **designing a system**. By using interfaces and private setters, you are making your backend resilient to change—a core requirement for any Intune-adjacent service.

