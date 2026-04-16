# Day 05: .NET & ASP.NET Core Fundamentals

Today is the milestone where you stop writing "code" and start building "services." We are moving from the console into the browser/HTTP world. For your Microsoft internship, this is the environment where you will spend 90% of your time.

---

## 🎯 Learning Objectives
* **The "Stack" Hierarchy:** Clearly distinguish between C#, .NET, and ASP.NET Core.
* **The Startup Brain:** Master the role of `Program.cs` in composing an app.
* **The Pipeline:** Understand how a request travels from a URL to your C# method.
* **Controller Anatomy:** Map HTTP verbs (GET, POST) to backend actions.
* **Initial Web API:** Transition **Endpoint Guardian** into a real HTTP service.

---

## 🧠 The Mental Model: What are you actually using?

It’s easy to mix these up. Think of it as a Russian Nesting Doll:

| Layer | What it is | Example |
| :--- | :--- | :--- |
| **C#** | The **Language** | `public class Device { ... }` |
| **.NET** | The **Platform & Runtime** | The engine that runs the code and the Base Class Library. |
| **ASP.NET Core** | The **Web Framework** | The tools that handle HTTP requests, routing, and JSON. |



---

## 🚀 The Request Lifecycle
When an admin in the Intune dashboard clicks "Evaluate," a request hits your backend. Here is the path it takes:

1.  **Entry:** The request hits the port your app is listening on.
2.  **Middleware Pipeline:** The request passes through "filters" (logging, auth, etc.).
3.  **Routing:** The system looks at the URL (e.g., `/api/devices/123`) and finds the matching Controller.
4.  **Model Binding:** The system takes the JSON from the request and turns it into a C# object.
5.  **Action:** Your method (e.g., `Evaluate()`) runs.
6.  **Response:** You return an `IActionResult` (like `Ok` or `NotFound`), which gets turned back into JSON.



---

## 📂 The Anatomy of a Controller
In enterprise systems, we use **Controllers** to group related tasks. For **Endpoint Guardian**, we use a `DevicesController`.

```csharp
[ApiController]           // Tells ASP.NET this is an API, not a website
[Route("api/[controller]")] // Automatically maps to "api/devices"
public class DevicesController : ControllerBase
{
    // Constructor Injection: Getting our tools (Evaluator) from the app
    private readonly IComplianceEvaluator _evaluator;
    public DevicesController(IComplianceEvaluator evaluator) => _evaluator = evaluator;

    [HttpGet("{id}")] // Handles GET api/devices/dev-001
    public IActionResult GetById(string id) 
    {
        // ... logic to find device ...
        return Ok(device); // Returns 200 OK with JSON
    }
}
```



---

## 🏗️ Building Endpoint Guardian (API v1)

### The First REST Surface
For this internship prep, we are moving away from "Console.WriteLine" and toward these standardized routes:

* `GET /api/devices` — List all registered devices.
* `GET /api/devices/{id}` — Get details for one device.
* `POST /api/devices` — Register a new device.
* `POST /api/devices/{id}/evaluate` — **The Core Logic:** Trigger a compliance check.

### Why `CreatedAtAction`?
When you create a device (`POST`), don't just return `200 OK`. Return `201 Created`. It tells the caller, "I made it, and here is where you can find it in the future." This is "boring, predictable, professional" API design.

---

## 🛠️ Build Tasks & Exercises

### [ ] Build Task: The Web Transition
1.  Initialize a new Web API project: `dotnet new webapi -n EndpointGuardian.Api`.
2.  Move your `ManagedDevice` and `IComplianceEvaluator` into the `Models` and `Services` folders.
3.  Implement the `DevicesController` with the 4 routes listed above.
4.  **Test it:** Use `curl` or a REST client to POST a device and then call the `/evaluate` endpoint.

### [ ] Mini Exercises
* **Exercise 1:** In `Program.cs`, why do we need `builder.Services.AddControllers()`? (Hint: It's the "Service Registration" phase).
* **Exercise 2:** Create a new endpoint `GET /api/devices/noncompliant` that uses **LINQ** to return only failing devices.
* **Exercise 3:** What is the difference between a **Path Parameter** (`{id}`) and a **Request Body**?

---

## 📚 Reference Documentation
* [ASP.NET Core Fundamentals](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/)
* [Routing in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing)
* [Controller-based Web APIs](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

---
> **Day 5 Recap:** You’ve graduated from "code" to "platform." You now have a running web server that understands your domain logic. Tomorrow, we dive into **Dependency Injection (DI)**—the glue that holds Microsoft-scale applications together. 

