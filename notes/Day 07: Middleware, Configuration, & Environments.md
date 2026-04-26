# Day 07: Middleware, Configuration, & Environments

## 🎯 Learning Objectives
* **The Request Pipeline:** Master the "Inward-Outward" flow of Middleware.
* **Flow Control:** Understand the distinct roles of `Use`, `Run`, and `Map`.
* **The Options Pattern:** Transition from string-based config to strongly typed classes.
* **Environment Awareness:** Differentiate behavior across Development, Staging, and Production.

---

## 🛣️ The Middleware Pipeline (The Toll Booths)
Middleware is software assembled into an app pipeline to handle requests and responses. In an Intune-style backend, this is where you handle **cross-cutting concerns** (security, logging, diagnostics) so they don't clutter your business logic.



### 1. `Use`, `Run`, and `Map`
| Delegate | Behavior | Best Use Case |
| :--- | :--- | :--- |
| **`Use`** | Can do work before and after the next middleware. | Logging, Exception handling, Auth. |
| **`Run`** | **Terminal.** Ends the pipeline; nothing runs after this. | Simple health checks or "Hello World" stubs. |
| **`Map`** | **Branching.** Creates a side-road based on the URL path. | `/health` or `/metrics` endpoints. |

### 2. Why Order Matters
Middleware runs in the order it is added. If you want to catch an exception that happens in a controller, the **Exception Middleware** must be the very first toll booth on the highway. 

> **Rule of Thumb:** Exception Handling → HSTS/HTTPS → Static Files → Routing → Auth → Endpoints.

---

## ⚙️ Configuration & The Options Pattern
Configuration in ASP.NET Core is a layered system of providers. By default, **Environment Variables** override `appsettings.json`, allowing you to keep secrets out of source control.



### The "Senior Move": The Options Pattern
Instead of reading raw strings like `_config["Settings:Key"]`, we map JSON sections to C# classes. This gives us:
1. **Type Safety:** You can't accidentally treat a string as an int.
2. **DI-Friendliness:** Inject `IOptions<T>` directly into your services.
3. **Clean Code:** Your business logic doesn't care *where* the settings came from.



---

## 🌍 Environments: Dev vs. Prod
ASP.NET Core treats environments as first-class citizens.
* **Development:** High-visibility (Developer Exception Pages, Swagger, Seed Data).
* **Production:** High-security (Generic error messages, HSTS, Secure Secret Stores).

> **Note:** `launchSettings.json` is for **local development only**. It is never deployed to the server. Use environment variables on the actual server (Azure/AWS) to control behavior there.

---

## 🏗️ Refactoring Endpoint Guardian (v2)

### 1. Centralized Exception Handling
We no longer use `try/catch` in every controller. We catch everything at the "front door":
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>(); // Toll booth #1
```

### 2. Strongly Typed Compliance Policy
Our `DeviceService` now receives its thresholds from `appsettings.json` via `IOptions<CompliancePolicyOptions>`.
```csharp
public DeviceService(IOptions<CompliancePolicyOptions> options) {
    _maxAge = options.Value.MaxCheckInAgeHours;
}
```

---

## 🛠️ Build Tasks & Exercises

### [ ] Build Task: The Pipeline Upgrade
1.  Add `RequestTimingMiddleware` to log how long every API call takes.
2.  Move your hardcoded compliance values into `appsettings.json`.
3.  Implement a `/health` branch using `app.Map()`.

### [ ] Mini Exercises
* **Exercise 1:** Why should `ExceptionHandlingMiddleware` be the very first toll booth?
* **Exercise 2:** What happens if a middleware doesn't call `await next()`? (Hint: Short-circuiting).
* **Exercise 3:** Create an `appsettings.Development.json` that sets a more "relaxed" compliance threshold for local testing.

---
> **Day 7 Recap:** Your controller is the destination, but the **Pipeline** is the journey. Professional engineering is about making that journey safe, observable, and configurable.

---
