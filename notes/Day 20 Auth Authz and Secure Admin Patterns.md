# Day 20 — Auth/Authz and Secure Admin Patterns

This is where **Endpoint Guardian** transitions from a public utility into a locked-down, enterprise-grade security platform. Up until now, anyone with your API URL could change a global policy. Today, we put a guard at the gate.

---

## 🎯 Learning Objectives
* Explain **Authentication** (Who) vs **Authorization** (What) clearly.
* Understand **JWT Bearer Authentication** and how to validate tokens in ASP.NET Core.
* Design a **Permission-Based** model inspired by Microsoft Graph.
* Implement **Policy-Based Authorization** using granular claims.
* Extract user identity (**Actor ID**) safely from validated tokens.

---

## 🏗️ 1. The Mindset: Security is Not an "Add-On"

In a real security platform, we don't just "add a login." We design every workflow around the **Principle of Least Privilege**. This means a "Dashboard Viewer" shouldn't have the same keys as a "Policy Admin."



### Core Definitions:
* **Authentication (AuthN):** "Who are you?" (e.g., Sahil presents a valid passport).
* **Authorization (AuthZ):** "What are you allowed to do?" (e.g., Sahil's passport has a 'Visa' allowing him to enter the 'Policy' room).

---

## 🎫 2. Claims: The Building Blocks of Identity

In a token-based system, your identity isn't just a username. It’s a collection of **Claims**—verified statements about you.

| Claim Type | Example | Purpose |
| :--- | :--- | :--- |
| **sub** | `user-001` | The unique ID of the user (Subject). |
| **name** | `Sahil` | The friendly display name. |
| **permission** | `policies.write` | A specific "key" to an API action. |



> **Security Rule:** Never trust a User ID sent in the request body (e.g., `{ "assignedBy": "admin" }`). Always extract the "Actor" from the claims inside the **digitally signed** token.

---

## 🛠️ 3. The Endpoint Guardian Permission Model

Instead of one giant "Admin" role, we are using fine-grained permissions. This allows for **Graph-style** thinking:

* `devices.read` / `devices.write`
* `policies.read` / `policies.write`
* `assignments.write`
* `evaluations.run`
* `access.decide`

### Suggested Roles:
* **ReadOnlyOperator:** `devices.read`, `policies.read`.
* **PolicyAdmin:** Everything related to policies and assignments.
* **ComplianceOperator:** Permission to run evaluations and view results.

---

## ⚙️ 4. Configuring JWT in `Program.cs`

We use the **JwtBearer** package to tell our app how to "read and verify" the incoming passports.

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)),
            ValidateLifetime = true
        };
    });
```



### Defining the "Gatekeepers" (Policies)
We map our claims to specific **Policies** that we can then apply to our Controllers.

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanWritePolicies", policy => 
        policy.RequireClaim("permission", "policies.write"));
    
    options.AddPolicy("CanReadDevices", policy => 
        policy.RequireClaim("permission", "devices.read"));
});
```

---

## 🚀 5. Protecting the Controllers

Once the gatekeepers are defined, protecting an endpoint is as simple as adding an attribute.

```csharp
[ApiController]
[Route("api/policies")]
public class PoliciesController : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "CanWritePolicies")] // Only those with the "write" key get in
    public async Task<ActionResult<PolicyResponse>> Create(CreatePolicyRequest request)
    {
        // ... logic ...
    }
}
```

---

## 🛠️ 6. The Dev Token Generator

Since we aren't using a full Identity Provider (like Entra ID) for this local project, we built a **Dev Token Generator**. This is a "Token Bakery" that lets us mint test tokens with different permissions.

```csharp
if (app.Environment.IsDevelopment())
{
    app.MapPost("/dev/token", (CreateDevTokenRequest request, IConfiguration config) =>
    {
        // ... Logic to bake a signed JWT token ...
        return Results.Ok(new { Token = tokenString });
    });
}
```

> **Warning:** This is for development **only**. In production, a hacker could use this to give themselves "God Mode" access.

---

## 🚦 7. Testing the Outcomes

| HTTP Status | Meaning | Scenario |
| :--- | :--- | :--- |
| **401 Unauthorized** | "I don't know who you are." | No token provided or token is invalid. |
| **403 Forbidden** | "I know you, but you don't have the key." | Valid token, but missing the required `permission` claim. |
| **200/201 Success** | "Access Granted." | Valid token with the correct permission claim. |

---

## 🚩 8. Common Pitfalls to Avoid

1.  **Middleware Order:** Always call `app.UseAuthentication()` **before** `app.UseAuthorization()`. You can't authorize someone you haven't identified yet!
2.  **Hardcoded Secrets:** Never commit your `SigningKey` to GitHub. Use User Secrets or Key Vault.
3.  **Client-Supplied Identity:** Never trust the client to tell you their username. If `User.GetActorId()` says they are "Sahil," they are Sahil—even if they claim to be "Admin" in the JSON body.

---

## 🏁 End-of-Session Recap
Today, you shifted from **building features** to **engineering trust**. By implementing policy-based authorization, you’ve ensured that **Endpoint Guardian** follows the same security principles as the world's largest cloud platforms.

**Next Up: Day 21 — Remediation and Audit Events.** Now that we know *who* is calling our API, we're going to keep a permanent record of every important action they take.

---
