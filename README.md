# Case Management & Audit System - EF Core Reference Project

An enterprise .NET 8 reference solution demonstrating modern **Entity Framework Core** concepts, ready to open in **Visual Studio 2022**, **VS Code**, or **JetBrains Rider**.

---

## What This Project Demonstrates

1. **Pure Fluent API Configuration (`Data/Configurations/`)**
   - Independent `IEntityTypeConfiguration<T>` classes for every entity.
   - Composite keys for many-to-many relationship (`CaseAssignee`).
   - Composite indexes on `(Status, Priority)` for fast query filtering.
   - Enum-to-string mapping via `HasConversion<string>()`.
   - Concurrency tokens with `IsRowVersion()`.

2. **Automated Auditing via Interceptor (`Data/Interceptors/AuditSaveChangesInterceptor.cs`)**
   - Subclasses `SaveChangesInterceptor`.
   - Automatically intercepts `SavingChanges` and `SavingChangesAsync`.
   - Detects state changes (`Added`, `Modified`, `Deleted`) on `IAuditableEntity`.
   - Computes a JSON diff of modified properties (original vs. current values) and appends an immutable record to `AuditLogs` in the same database transaction.

3. **High-Performance Querying vs Cartesian Explosion (`Endpoints/CaseEndpoints.cs`)**
   - `AsNoTracking()` with lightweight projections for read queries.
   - Demonstration of `AsSplitQuery()`: compares a single multi-table Cartesian JOIN against multiple optimized SELECT queries when loading parent records with multiple child collections (`Notes`, `Documents`, `Assignees`).

4. **Direct Database Bulk Updates (`ExecuteUpdateAsync`)**
   - Bulk archives resolved cases older than a given date directly on the database engine without materializing entities into C# memory.

---

## How to Run in Visual Studio

1. Open `CaseManagementAudit.sln` in Visual Studio 2022.
2. Set `CaseManagementAudit` as the Startup Project.
3. Press **F5** (Debug) or **Ctrl+F5** (Run).
4. The application starts, creates `casemanagement.db` (SQLite) automatically, seeds 3 realistic cases, and opens **Swagger UI** in your browser at `http://localhost:<port>/`.