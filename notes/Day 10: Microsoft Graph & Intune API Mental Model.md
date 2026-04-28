# Day 10: Microsoft Graph & Intune API Mental Model

## 🎯 Learning Objectives
* **Unified Boundary:** Understand Graph as the "Front Door" to Microsoft Cloud workloads.
* **Intune Integration:** Map Intune product concepts to Graph resources.
* **Permission Models:** Deep dive into Delegated vs. Application access.
* **API Maturity:** Recognize the importance of Versioning (v1.0 vs. Beta).

---

## 🏛️ What is Microsoft Graph?
Graph is a **Unified Gateway**. Instead of having 50 different APIs for 50 different services, Microsoft provides one endpoint (`graph.microsoft.com`) that routes your request to the correct internal service (Intune, Entra ID, Outlook, etc.).



> **The "Front Door" Analogy:** Graph is the receptionist at the front desk of Microsoft. You give them your ID and tell them who you want to see. They handle the security check and lead you to the right office (Intune). They are *not* the person doing the actual work in the office; they are the interface.

---

## 🛡️ The Trust Model: Delegated vs. Application
This is the most critical security concept in the Microsoft ecosystem. Choosing the wrong one can lead to massive security vulnerabilities.



| Feature | **Delegated Permissions** | **Application Permissions** |
| :--- | :--- | :--- |
| **Identity** | App + Signed-in User. | App only (Service Identity). |
| **Analogy** | **The Assistant:** Acts only on what *you* can do. | **The Night Guard:** Has their own master key. |
| **User Present?** | **Yes.** A human must log in. | **No.** Runs in the background (Daemon). |
| **Trust Level** | Limited by the human's permissions. | Powerful; requires Global Admin consent. |

---

## 🔧 Resources & Actions: Beyond CRUD
In Intune, we don't just "Update" a device. We **interact** with it. Graph uses a mix of resource nouns and action verbs.

1.  **Resources (Nouns):** `managedDevice`, `deviceComplianceScript`, `mobileApp`.
2.  **Actions (Verbs):** `wipe`, `retire`, `reboot`, `sync`.



**Why this matters for Endpoint Guardian:** Our API should follow this. Instead of `PUT /devices/123` (which suggests changing the device name), we should use `POST /devices/123/reboot` (which triggers a workflow).

---

## 🧪 Versioning: v1.0 vs. Beta
* **v1.0:** The "Contract." These APIs are stable and will not break.
* **Beta:** The "Sandbox." These have the newest features but can change tomorrow without notice.

**Internship Tip:** If you're building a feature and notice it uses a Beta endpoint, call it out! It shows you understand **Production Readiness**.

---

## 🏗️ Refactoring Project Design: "The Graph Way"

### 1. Updated Resource Map
Instead of generic endpoints, we will structure **Endpoint Guardian** to feel like a slice of Graph:
* `GET /api/devices` (List `managedDevices`)
* `POST /api/devices/{id}/check-ins` (Trigger a check-in event)
* `POST /api/devices/{id}/evaluations` (Generate a compliance report)

### 2. Permission Personas
We will design our future security around these roles:
* **Device.Read.All:** For a "Compliance Viewer" (Delegated).
* **DeviceManagementManagedDevices.PrivilegedOperations.All:** For an "Admin" (Delegated).
* **Service.Automation:** For our background cleanup workers (Application).

---

## 🛠️ Build Task for Day 10 (Documentation)
Since today is conceptual, your "code" is actually a **Design Update** in your repo.
1.  **Update `README.md`**: Add a "Graph-Inspired Architecture" section.
2.  **Resource Map**: Document the 5 core "Nouns" and 3 "Actions" your API supports.
3.  **Governance Note**: Write a paragraph explaining how you would handle `v1.0` vs `Beta` if your project had breaking changes.

---
> **Day 10 Recap:** You are no longer just building a backend; you are building an **Enterprise-Grade Service Boundary**. By adopting Graph's vocabulary and permission models, you are speaking the language of Microsoft.
