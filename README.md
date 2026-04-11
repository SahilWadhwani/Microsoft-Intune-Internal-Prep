# 🚀 Road to Redmond: Microsoft Intune Prep

A 30-day intensive sprint documenting my journey from a Python/Go background into the **Microsoft Intune** engineering ecosystem. This repository serves as a knowledge base for Intune product domains and the home of **Endpoint Guardian**—a policy-driven device compliance service.

## 📌 The Mission

To bridge the gap between graduate-level CS fundamentals and the engineering realities of an enterprise-scale security product. This involves:

  * **Domain Ramp-up:** Understanding Endpoint Management, Zero Trust, and Compliance.
  * **Stack Translation:** Mapping backend instincts from Python/Go/FastAPI to **C\#** and **ASP.NET Core**.
  * **Engineering Excellence:** Implementing observability, auditability, and deterministic policy evaluation.

-----

## 🏗️ The Project: Endpoint Guardian

**Endpoint Guardian** is a backend service built to simulate the core mechanics of Microsoft Intune. It focuses on the lifecycle of a managed device—from registration to compliance enforcement.

### Core Features

  * **Device Inventory:** Managed resource modeling for diverse endpoints.
  * **Policy Engine:** Deterministic evaluation of device posture (Encryption, OS version, etc.).
  * **Action Orchestration:** Triggering remediation and admin-facing reporting.
  * **Graph-Ready:** Designed with an API surface inspired by Microsoft Graph.

**Tech Stack:** .NET 10, ASP.NET Core, EF Core, SQLite, and Structured Logging (Serilog).

-----

## 📂 Repository Structure

```text
.
├── notes/                   # Daily "Masterclass" summaries and deep dives
│   ├── day-01-intune-core.md
│   └── ...
├── endpoint-guardian/       # The core implementation (C# / ASP.NET Core)
│   ├── src/                 # Domain logic, API controllers, and Services
│   └── tests/               # Unit/Integration tests for the Policy Engine
├── docs/                    # Architecture diagrams and ADRs (Decision Records)
└── README.md                # You are here
```

-----

## 🗓️ Roadmap (April 11 – May 10)

| Phase | Focus | Status |
| :--- | :--- | :--- |
| **Phase 1** | **Foundations:** Product Pillars, C\# Basics, & DI | 🏗️ In Progress |
| **Phase 2** | **Core Backend:** API Design, Domain Modeling, & State | ⏳ Pending |
| **Phase 3** | **Compliance Engine:** Logic, EF Core, & Edge Cases | ⏳ Pending |
| **Phase 4** | **Quality & Polish:** Observability, Refactoring, & Demo | ⏳ Pending |

-----

## 📖 Latest Knowledge Logs

  * **[Day 01: Thinking Like an Intune Engineer](https://www.google.com/search?q=./notes/day-01-intune-core.md)**
      * Defining the "Zero Trust" mindset.
      * Mapping FastAPI concepts to ASP.NET Core.
      * Identifying the 6 core entities of Endpoint Guardian.

-----

## 🛠️ Setup & Usage

*(Instructions will be updated as the project moves into Phase 2)*

1.  Clone the repo.
2.  Ensure .NET 10 SDK is installed.
3.  `dotnet run --project endpoint-guardian`

-----

**Goal:** Be "Internship-Ready" by May 18, 2026.
*“It is not about learning a framework; it is about learning a product-service-security system.”*