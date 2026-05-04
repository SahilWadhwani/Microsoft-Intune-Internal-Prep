# Day 18: Persistence with EF Core + SQLite

Today is a major realism upgrade for **Endpoint Guardian**. We are moving from "RAM-only" storage to a **real database**. This ensures your data survives an app restart and follows professional .NET persistence patterns.

---

## 🎯 Learning Objectives
* Understand **EF Core** as the modern object-database mapper for .NET.
* Configure **SQLite** for lightweight, file-based local storage.
* Implement the **Async/Await** pattern across the entire data stack.
* Manage schema changes using **EF Core Migrations**.

---

## 🏗️ 1. Core Concepts: The Persistence Stack

### What is EF Core?
EF Core is an **ORM (Object-Relational Mapper)**. It allows you to work with database data using C# objects and LINQ, while it handles the translation into SQL.



### Why SQLite?
* **Lightweight:** No server installation required (Postgres/SQL Server are overkill for local dev).
* **Portable:** Stores everything in a single `.db` file.
* **Professional:** Supported by Microsoft as a first-class provider for EF Core.

### The `DbContext`
Think of the `DbContext` as a **Unit of Work**. It represents a single session with the database.
> **Important Rule:** `DbContext` is **Scoped**. It is created when an API request starts and disposed of when it ends. This prevents memory leaks and data tracking bugs.

---

## 🛠️ 2. Setting Up the Database

### The Connection String
Add this to your `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "EndpointGuardianDb": "Data Source=endpointguardian.db"
  }
}
```

### The `DbContext` Configuration
Your `EndpointGuardianDbContext` maps your Domain Entities to Database Tables.



```csharp
public class EndpointGuardianDbContext : DbContext
{
    public DbSet<ManagedDevice> Devices => Set<ManagedDevice>();
    public DbSet<CompliancePolicy> Policies => Set<CompliancePolicy>();
    public DbSet<PolicyAssignment> PolicyAssignments => Set<PolicyAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Example: Mapping Enums as Strings for better readability in SQLite
        modelBuilder.Entity<ManagedDevice>()
            .Property(d => d.Platform)
            .HasConversion<string>();

        // Mapping Value Objects (Owned Entities)
        modelBuilder.Entity<ManagedDevice>()
            .OwnsOne(d => d.CurrentPostureSnapshot);
    }
}
```

---

## 🚦 3. Modernizing the Data Layer: Async/Await
Because talking to a database is an **I/O-bound** operation, we must refactor our repositories to be asynchronous.

### Updated Repository Interface
```csharp
public interface IDeviceRepository
{
    Task<List<ManagedDevice>> GetAllAsync();
    Task<ManagedDevice?> GetByIdAsync(string id);
    Task AddAsync(ManagedDevice device);
    Task UpdateAsync(ManagedDevice device);
}
```

### Async Implementation Example
```csharp
public async Task AddAsync(ManagedDevice device)
{
    await _dbContext.Devices.AddAsync(device);
    await _dbContext.SaveChangesAsync(); // The actual SQL 'INSERT' happens here
}
```

---

## 🔍 4. Performance: `IQueryable` vs `IEnumerable`
When filtering data, we want the **Database** to do the work, not the C# App.

| Feature | `IEnumerable` | `IQueryable` |
| :--- | :--- | :--- |
| **Execution** | Filters in App RAM. | Filters in the Database (SQL). |
| **Performance** | Slow for large datasets. | Fast; only fetches what is needed. |
| **EF Core Usage** | `.AsEnumerable()` | `.AsQueryable()` |



---

## 🚜 5. Managing the Schema: Migrations
Your C# code and SQL database are now "married." If you change one, you must update the other.

1.  **Change Model:** Add a property to your C# class.
2.  **Add Migration:** `dotnet ef migrations add AddNewProperty`.
3.  **Update Database:** `dotnet ef database update`.

---

## 🏁 End-of-Session Recap
* **Persistence** means your project can now scale and survive restarts.
* **Async/Await** keeps your API responsive while waiting for the disk.
* **Scoped Lifetimes** for `DbContext` and Repositories are the industry standard for .NET APIs.
* **Migrations** allow you to version-control your database schema alongside your code.

**Next Up:** **Day 19 — Testing Fundamentals.** We will make this "Production-Ready" by adding Unit Tests for our compliance engine.

---
