
# Day 12: Build Device Onboarding & Inventory APIs

## 🎯 Learning Objectives
By the end of today, you will:
* Differentiate between **Onboarding** (Resource Creation) and **Inventory** (Resource Querying).
* Implement clean **DTO mapping** to separate Domain Models from API Responses.
* Support **Filtering and Pagination** to handle enterprise-scale datasets.
* Master the use of `CreatedAtAction` and specific HTTP status codes (**201**, **404**, **409**).

---

## 💡 1. The Mindset Shift: Onboarding vs. Inventory
In an Intune-style system, devices aren't just "rows in a table." They are managed resources with a lifecycle.

* **Onboarding:** Introducing a device to the system, validating its initial posture, and generating a unique identity.
* **Inventory:** The admin’s "Search Engine." It requires list-friendly projections, paging, and filtering so the UI doesn't crash under 100,000 devices.



---

## 🛠 2. The API Contract
For Day 12, we are building a resource-oriented surface that follows Microsoft's REST API guidelines.

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| **POST** | `/api/devices` | **Onboard:** Register a new managed device. |
| **GET** | `/api/devices` | **Inventory:** List devices with filtering and paging. |
| **GET** | `/api/devices/{id}` | **Detail:** Fetch full information for one device. |

---

## 📦 3. Data Transfer Objects (DTOs)
We use different DTOs for **Detail** vs. **Summary** views. This optimizes performance and prevents "leaking" internal data.

### Request & Query Models
```csharp
public record CreateDeviceRequest(
    string DeviceName,
    DevicePlatform Platform,
    int OsVersion,
    bool? IsEncrypted,
    bool? HasPassword,
    bool? DefenderEnabled
);

public record GetDevicesQuery(
    DevicePlatform? Platform = null,
    ComplianceStatus? Status = null,
    int Page = 1,
    int PageSize = 20
);
```

### Response Models
```csharp
public record DeviceResponse(
    string Id,
    string DeviceName,
    DevicePlatform Platform,
    int OsVersion,
    bool? IsEncrypted,
    bool? HasPassword,
    bool? DefenderEnabled,
    DateTime LastCheckInUtc,
    ComplianceStatus? CurrentComplianceStatus
);

public record PagedDevicesResponse(
    List<DeviceSummaryResponse> Items,
    int Page,
    int PageSize,
    int TotalCount
);
```

---

## 🏗 4. Implementation Logic

### Controller Design
The controller should remain "thin," delegating logic to the service and returning the correct `ActionResult<T>`.

```csharp
[HttpPost]
public ActionResult<DeviceResponse> Create(CreateDeviceRequest request)
{
    var result = _deviceService.CreateDevice(request);
    if (result is null)
    {
        return Conflict("A device with this identity already exists.");
    }
    // Returns 201 Created with a Location header pointing to GetById
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}
```

### Service Logic: Filtering & Pagination
The service handles the "Heavy Lifting" of narrowing down the data slice.

```csharp
public PagedDevicesResponse GetDevices(GetDevicesQuery query)
{
    var devices = _deviceRepository.GetAll().AsEnumerable();

    // 1. Filtering
    if (query.Platform is not null)
        devices = devices.Where(d => d.Platform == query.Platform);

    if (query.Status is not null)
        devices = devices.Where(d => d.CurrentComplianceStatus == query.Status);

    var totalCount = devices.Count();

    // 2. Sorting & Paging
    var items = devices
        .OrderBy(d => d.DeviceName)
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .Select(ToSummaryResponse)
        .ToList();

    return new PagedDevicesResponse(items, query.Page, query.PageSize, totalCount);
}
```



---

## 🚦 5. Status Code Discipline
Don't just return `200 OK`. Be deliberate to help automation and scripts.

* **201 Created:** Use after `POST /api/devices`. Always include the `Location` header.
* **404 Not Found:** When a requested ID doesn't exist.
* **409 Conflict:** When trying to create a device that already exists (identity collision).
* **400 Bad Request:** When pagination parameters are out of range (e.g., `Page < 1`).

---

## 📝 6. Exercises & Build Tasks
1.  **DTO Refactor:** Implement the `CreateDeviceRequest`, `DeviceResponse`, and `PagedDevicesResponse`.
2.  **Mapping:** Write `private static` helper methods in your service to map Domain Entities to DTOs.
3.  **The "Inventory" Challenge:** Ensure `GET /api/devices` can filter by **Platform** and **Status** simultaneously.
4.  **Validation:** In your onboarding flow, ensure `DeviceName` is not empty and `OsVersion` is greater than 0.

---

## 🏁 End-of-Session Recap
* **Onboarding** establishes identity; **Inventory** manages discovery.
* Separating **Summary vs. Detail** responses is a senior-level performance habit.
* **Pagination** is not optional—it’s how we build for the "100k Device" scenario.
* **Thin Controllers + Rich Services** = Maintainable Architecture.

**Next Up:** **Day 13 — Build Policy Management APIs.** We’ll add the second major pillar of Endpoint Guardian: managing the rules that govern our devices.
