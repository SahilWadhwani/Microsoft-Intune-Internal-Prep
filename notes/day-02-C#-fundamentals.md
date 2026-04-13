# Day 02: C# Fundamentals for Backend Engineers

## 🎯 Learning Objectives
* **Mental Shift:** Moving from "Python with semicolons" to explicit, typed C#.
* **Type Mastery:** Understanding the Value vs. Reference type split.
* **The "Null" Strategy:** Using nullability to represent "Missing Telemetry" vs. "False."
* **Domain Modeling:** Building the first iteration of `ManagedDevice` and `CompliancePolicy`.

---

## 🧠 The Mental Model: C# vs. Python/Go
In a security-heavy product like Intune, logic depends on **exact states**. Vague dictionaries won't cut it.

| Feature | Python | Go | C# |
| :--- | :--- | :--- | :--- |
| **Typing** | Dynamic (Loose) | Static (Strict) | **Static & Explicit** |
| **Modeling** | Dictionaries | Structs | **Classes / Records** |
| **Nulls** | `None` | Pointers/Nil | **Nullable Reference Types** |

---

## 💎 Core Concepts: The "So What?" for Intune

### 1. Value vs. Reference Types
This is the most common source of state-leaking bugs in backend systems.

* **Value Types (`int`, `bool`, `DateTime`):** Stored directly. Changing a copy doesn't change the original.
* **Reference Types (`string`, `class`, `List`):** Variables store a *pointer* to an object. Changing `device2.Name` will change `device1.Name` if they point to the same object.



### 2. The Power of Nullability (`?`)
In Intune, "Missing Data" is a business state. 

* `bool EncryptionEnabled = false;` → We **know** it's off. (Fail compliance).
* `bool? EncryptionEnabled = null;` → The device **hasn't reported yet**. (Trigger a "Warning" or "Needs Sync").

> **Backend Pro-Tip:** Use `?` whenever "Unknown" or "Stale" is a valid scenario. Flattening `null` into `false` too early leads to incorrect compliance reporting.



### 3. String Interpolation
Readable logs are non-negotiable at Microsoft-scale.
```csharp
// Better than concatenation
Console.WriteLine($"Device {device.Id} is {status} as of {lastSync}.");
```

---

## 🏗️ Domain Model: Endpoint Guardian (v1)

We aren't building a generic "Todo" app. We are modeling a **Policy Evaluation System**.

### The `ManagedDevice` Class
```csharp
public class ManagedDevice
{
    public string Id { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public int OsVersion { get; set; }
    public bool? IsEncrypted { get; set; }  // Nullable: Has it checked in?
    public bool? DefenderEnabled { get; set; }
    public DateTime LastCheckInUtc { get; set; }
}
```

### The `GetComplianceFailures` Logic
Notice the use of `!= true`. This handles the `null` state safely. If the value is `false` OR `null`, we add a failure reason.

```csharp
public static List<string> GetComplianceFailures(ManagedDevice device, CompliancePolicy policy)
{
    var failures = new List<string>();

    if (policy.RequireEncryption && device.IsEncrypted != true)
    {
        failures.Add("Device encryption is not enabled or not reported.");
    }
    
    // ... other checks
    return failures;
}
```

---

## 🛠️ Build Tasks & Exercises

### [ ] Build Task: The Console Evaluator
1. Create a project `EndpointGuardian.Day2`.
2. Implement the `ManagedDevice` and `CompliancePolicy` classes.
3. Write a `foreach` loop to evaluate a list of 3 devices (one compliant, two non-compliant).
4. **Expected Output:**
   ```text
   Device: Sales-Laptop-07
   Result: NonCompliant
   Reasons:
   - Device encryption is not enabled.
   - Device check-in is stale.
   ```

### [ ] Mini Exercises
* **Exercise 1:** Write a method `static bool RequiresRemediation(bool isCompliant)` that returns `true` when the device is failing.
* **Exercise 2:** Create a `CompliancePolicy` class with `MinimumOsVersion` and `MaxCheckInAgeHours`.
* **Exercise 3:** Why would we use `decimal` instead of `double` for a financial field in a backend service?

---

## 📚 Reference Documentation
* [C# Type System Basics](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/types/)
* [Nullable Value Types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types)
* [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

---
> **Day 2 Recap:** C# is explicit for a reason. By being strict with types and nulls now, we prevent massive headaches when we move to the API layer in Phase 2. **Boring, predictable code is professional code.**