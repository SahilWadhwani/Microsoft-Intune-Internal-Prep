
# Day 09: REST API Design for Admin/Platform Systems

## 🎯 Learning Objectives
* **Resource-First Design:** Move from "Verbs" (Actions) to "Nouns" (Resources).
* **DTO Separation:** Learn why the "Kitchen Model" should never be the "Plated Meal."
* **HTTP Semantics:** Master status codes beyond `200` and `404`.
* **Idempotency:** Ensure retries don't break your system.
* **List Shaping:** Implement filtering, sorting, and pagination for scale.

---

## 🏗️ The Core Mindset: Resources > Methods
Bad APIs are built around **what the code does** (RPC-style). Professional APIs are built around **the things that exist** (Resource-style).



| Bad (RPC-style) | Good (Resource-oriented) | Why? |
| :--- | :--- | :--- |
| `/api/GetAllDevices` | `GET /api/devices` | Stable nouns are easier to document. |
| `/api/CreateNewDevice` | `POST /api/devices` | Uses HTTP verbs to define the action. |
| `/api/DoComplianceCheck` | `POST /api/devices/{id}/evaluations` | Treats the check as a "recordable event." |

---

## 🍽️ DTOs vs. Domain Models
One of the most important architectural boundaries is between your **Internal Model** (ManagedDevice) and your **External DTO** (DeviceResponse).



* **Domain Models:** Contain sensitive internal state, database IDs, and business logic.
* **DTOs:** A "Contract" with the user. They only contain what the user needs to see and stay stable even if you rewrite your entire database.

---

## 🚦 The Language of Status Codes
Don't just return `200 OK` for everything. Use the right status code so automation scripts can react correctly without parsing your JSON.

| Status Code | Meaning | Use Case in Endpoint Guardian |
| :--- | :--- | :--- |
| **201 Created** | Success + New Resource. | Returning the URL of a newly registered laptop. |
| **204 No Content** | Success + Nothing to say. | After deleting a device or a policy. |
| **400 Bad Request** | Client Error. | Invalid platform name or negative OS version. |
| **409 Conflict** | Business Logic Error. | Trying to register a Device ID that already exists. |

---

## 🔄 Idempotency: The "Retry" Rule
In an enterprise system, networks are flaky. If an admin script times out while creating a device, it will try again.

* **Idempotency** means: *"If I hit this button 10 times, the result on the server is the same as if I hit it once."*
* **PUT** is typically idempotent (it replaces the resource).
* **POST** is typically NOT idempotent (it creates a new one every time).

> **Microsoft Habit:** For high-scale admin APIs, we often design "Create" actions to be idempotent by checking for existing IDs (using `409 Conflict` or simply returning the existing record).

---

## 📊 List Shaping (Filtering & Pagination)
In a real Intune environment, you might have **100,000 devices**. You cannot return them all in one array.

1.  **Filtering:** `GET /api/devices?platform=Windows`
2.  **Pagination:** `GET /api/devices?page=1&pageSize=50`



**The Professional Response Shape:**
```json
{
  "items": [...],
  "totalCount": 153,
  "page": 1,
  "pageSize": 50
}
```

---

## 🛠️ Build Tasks & Exercises

### [ ] Build Task: The Professional Surface
1.  **Rename Action Endpoints:** Change `/evaluate` to `POST /api/devices/{id}/evaluations`.
2.  **Implement DTOs:** Ensure no controller returns a `ManagedDevice` directly. Use `DeviceResponse` instead.
3.  **Add Pagination:** Update your `GET /api/devices` to accept `page` and `pageSize` as query parameters.
4.  **Use Correct Codes:** Ensure `POST` returns `201 Created` with a `Location` header.

### [ ] Mini Exercises
* **Exercise 1:** Why is `POST /evaluations` better than a simple `/runCheck` method?
* **Exercise 2:** What status code should you return if a user tries to delete a device that doesn't exist?
* **Exercise 3:** Create a `DeviceSummaryResponse` that only contains the ID, Name, and Compliance Status (to save bandwidth on list pages).

---
> **Day 9 Recap:** Your API is a product. By using stable nouns, clear status codes, and DTO boundaries, you are building a system that is safe for both humans and automation to use.

