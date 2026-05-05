# Day 19: Testing Fundamentals — Protecting the Core

Today, we transition from building features to **protecting** them. In a high-stakes environment like **Microsoft Intune**, "It works on my machine" is never enough. We need an automated way to prove that our security logic remains correct even after we change the code months from now.

---

## 🎯 Learning Objectives
* Distinguish between **Unit**, **Integration**, and **Manual** tests.
* Set up a professional **xUnit** project with **FluentAssertions**.
* Lock in the "Decision Matrix" for compliance and access control.
* Test **EF Core** using **SQLite in-memory** (isolating the database).
* Use **`WebApplicationFactory`** to run "App-in-a-box" integration tests.

---

## 🏛️ 1. The Testing Pyramid
We don't test everything the same way. We want a broad base of fast tests and a narrow top of slow tests.

| Test Level | Scope | Speed | Value |
| :--- | :--- | :--- | :--- |
| **Unit Tests** | Single method/logic block. | ⚡ Lightning Fast | High (Logic Correctness) |
| **Integration**| Multiple parts (Controller + DB). | 🐢 Medium | High (System Flow) |
| **Manual (E2E)**| Full user flow in Swagger/Postman.| 🐌 Very Slow | Low (Repeatability) |



---

## 🛠️ 2. Setting Up the Test Project
Run these commands from your root folder to create a professional testing environment:

```bash
# 1. Create the project
dotnet new xunit -n EndpointGuardian.Tests
dotnet sln add EndpointGuardian.Tests/EndpointGuardian.Tests.csproj

# 2. Add reference to your API
dotnet add EndpointGuardian.Tests/EndpointGuardian.Tests.csproj reference EndpointGuardian.Api/EndpointGuardian.Api.csproj

# 3. Add professional testing tools
dotnet add EndpointGuardian.Tests package FluentAssertions
dotnet add EndpointGuardian.Tests package Moq
dotnet add EndpointGuardian.Tests package Microsoft.AspNetCore.Mvc.Testing
```

---

## ⚖️ 3. Testing the "Brain": The Compliance Evaluator
This is the most critical part of your system to test. We use the **Arrange-Act-Assert** pattern.



### Example: Testing "Missing Telemetry"
We want to prove that if a user **doesn't report** encryption status, they **fail** compliance.

```csharp
[Fact]
public void EvaluatePolicy_WhenEncryptionNotReported_ReturnsNonCompliant()
{
    // Arrange: Create a device with NULL encryption
    var device = new ManagedDeviceBuilder()
        .WithEncryption(null) // Telemetry is missing
        .Build();
    var policy = new PolicyBuilder()
        .WithRequireEncryption(true)
        .Build();

    // Act: Run the evaluator
    var result = _evaluator.EvaluatePolicy(device, policy);

    // Assert: Use FluentAssertions to check the outcome
    result.Status.Should().Be(ComplianceStatus.NonCompliant);
    result.FailureReasons.Should().Contain(r => r.Code == "ENCRYPTION_NOT_REPORTED");
}
```

---

## 🚦 4. Testing the Decision Matrix
Your `AccessDecisionService` is essentially a logic table. We must test every row in that table to ensure we never accidentally "Allow" a dangerous device.

| Condition | Risk | MFA | Expected Decision |
| :--- | :--- | :--- | :--- |
| NonCompliant Device | Low | N/A | **RequireRemediation** |
| Compliant Device | High | N/A | **Block** |
| Compliant Device | Medium| False| **RequireMfa** |
| Unknown Compliance | Low | True | **RequireFreshCheckIn** |

---

## 💾 5. EF Core: Testing without the "Real" Database
You shouldn't use your local `endpointguardian.db` for tests—it makes tests "dirty" and unreliable. Instead, we use **SQLite in-memory**.



**The Trick:** Using `"DataSource=:memory:"` creates a database that exists only in RAM for the duration of the test.

```csharp
private static EndpointGuardianDbContext CreateInMemoryDb()
{
    var connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();
    var options = new DbContextOptionsBuilder<EndpointGuardianDbContext>()
        .UseSqlite(connection).Options;
    var context = new EndpointGuardianDbContext(options);
    context.Database.EnsureCreated(); // Build the schema instantly
    return context;
}
```

---

## 🌐 6. Integration Tests: `WebApplicationFactory`
This is an ASP.NET Core superpower. It spins up your **entire app** in memory (routing, DI, middleware) so you can hit it with a real `HttpClient`.

**Pro-Tip:** To make this work, add `public partial class Program { }` to the bottom of your `Program.cs`. This "opens" the class so the test project can see it.

---

## 📝 7. Build Tasks for Today
1. **Setup:** Create the `EndpointGuardian.Tests` project.
2. **Evaluator Tests:** Write 5+ tests for `BasicComplianceEvaluator` (Happy path, OS too low, Encryption missing, etc.).
3. **Combiner Tests:** Extract the `CombinePolicyResults` logic into a separate class/method and test the priority order (`Error` > `NonCompliant`).
4. **Access Tests:** Test the decision matrix in `AccessDecisionService`.
5. **Persistence Tests:** Write one test that proves `EfDeviceRepository` can save and retrieve a device using the in-memory database.

---

## 🏁 End-of-Session Recap
* **Testing is Documentation:** Your tests tell other developers exactly how the system is *supposed* to behave.
* **Isolate Logic:** Use `NullLogger` and Fakes to test services without needing the whole world.
* **Deterministic Time:** If you can, start using an `IClock` abstraction to test "Stale Check-ins" reliably.
* **Confidence:** With a green `dotnet test` suite, you can refactor your code without fear.

**This is the mark of a true Senior Engineer, Sahil.** You aren't just building a feature; you're building a **guarantee**.

---
