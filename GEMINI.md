# SmartRestaurant — Permanent Project Instructions & Engineering Governance

This document establishes the permanent working rules, architectural principles, documentation standards, project memory strategy, and development workflow for all AI Agent sessions operating in the **SmartRestaurant** repository.

---

# 1. PROJECT IDENTITY

- **Project Name**: SmartRestaurant
- **Product Description**:
  SmartRestaurant is an enterprise-grade restaurant management platform designed to support diverse food service business models and scales—from independent cafés and boutique bistros to high-volume dining establishments and multi-branch restaurant chains.
- **Operational Scope**:
  - Restaurant and branch management.
  - Restaurant areas, dining rooms, and table layout management.
  - Visual restaurant floor plan designer (interactive canvas).
  - Digital menu, categories, product variants, and modifier management.
  - Table service and waiter workflow operations.
  - Real-time order capture and lifecycle management.
  - Intelligent kitchen order routing by prep station.
  - Kitchen Display System (KDS) & Bar Station monitoring.
  - Food preparation staging and delivery/runner tracking.
  - Cashier management, bill calculation, and split/merge operations.
  - Multi-channel payment processing (Cash, Card, Dynamic VietQR / Napas 24/7).
  - Business intelligence, operational reporting, and analytics.
  - Customer self-service QR ordering (planned for a future phase).
- **Core Design Qualities**: Extensibility, maintainability, strict security, low-latency performance, and operational resilience.

---

# 2. APPROVED TECHNICAL DIRECTION

The technical foundation for SmartRestaurant is established as follows:

| Layer / Component | Approved Technology |
| :--- | :--- |
| **Backend Core** | ASP.NET Core Web API |
| **Architecture Pattern** | Modular Monolith |
| **Database Engine** | Microsoft SQL Server |
| **ORM / Data Access** | Entity Framework Core (Code-First) |
| **Admin & Management Portal** | ASP.NET Core MVC + Razor |
| **POS Application** | Responsive Web Application |
| **Kitchen Display System (KDS)** | Responsive Web Application |
| **Cashier Terminal** | Responsive Web Application |
| **Client Scripting & Styling** | HTML5, CSS3, modern TypeScript / JavaScript |
| **Real-Time Communication** | ASP.NET Core SignalR |
| **Floor Plan Visual Designer** | Konva.js (or vetted equivalent canvas library) |
| **Background Processing** | .NET Worker Service (`BackgroundService`) or Hangfire |

### Framework Version Verification
Use a currently supported **.NET LTS version** when implementation begins. Before creating or altering project files, the Agent must inspect and verify the actual installed .NET SDKs (`dotnet --list-sdks`) and target framework version on the host environment.

### Core Architectural Principles
1. **Start with a Modular Monolith**: Package modules within cohesive boundaries inside a single deployable unit.
2. **No Unjustified Microservices**: Do not introduce microservices or distributed architectures without an explicit, approved Architectural Decision Record (ADR).
3. **Strict Module Boundaries**: Isolate module data models and internal logic. Communication between modules must pass through well-defined service contracts or domain events.
4. **Avoid Distributed Complexity**: Eliminate unnecessary RPC overhead, dual-write scenarios, and complex eventual-consistency choreography unless business scale mandates it.
5. **Early Multi-Tenant / Multi-Branch Isolation**: Design `OrganizationId` and `BranchId` isolation boundaries into database schemas and queries from the ground up to support future multi-location scaling.
6. **Centralized Backend Logic**: Keep business calculations (pricing, discounts, tax rates, inventory deduction, order state transitions) in backend application and domain layers. Never duplicate business logic on frontend clients.
7. **Evolvable Contracts**: Design public APIs, DTOs, and SignalR hub contracts with backward compatibility and versioning in mind.

> [!IMPORTANT]
> The above technical direction represents architectural boundaries, **NOT** permission to begin premature implementation.

---

# 3. MANDATORY PROJECT MEMORY SYSTEM

SmartRestaurant maintains persistent, version-controlled project context using Markdown files located in the `.agent-memory/` directory.

### Primary Memory Files
- `.agent-memory/README.md`: Index and operational instructions for project memory.
- `.agent-memory/PROJECT-STATUS.md`: High-level stage tracking, milestones, active risks, and open questions.
- `.agent-memory/ARCHITECTURE-DECISIONS.md`: Formal Architectural Decision Records (ADRs).
- `.agent-memory/CODEBASE-MAP.md`: Navigational catalog of projects, folders, entry points, and interfaces.
- `.agent-memory/DEVELOPMENT-LOG.md`: Chronological audit trail of executed tasks and verification outcomes.
- `.agent-memory/NEXT-TASK.md`: Authoritative specification for the single next approved task.
- `.agent-memory/KNOWN-ISSUES.md`: Technical debt, unresolved bugs, and operational caveats.

Documentation Index: `docs/00-index.md`  
Root Rule Anchor: `GEMINI.md`

## 3.1. Mandatory Context Restoration

At the start of **every** new task or CLI session, the Agent must restore context in the following sequence:

1. Read `GEMINI.md` (this file).
2. Read `.agent-memory/PROJECT-STATUS.md` (if it exists).
3. Read `.agent-memory/NEXT-TASK.md` (if it exists).
4. Read relevant approved decisions in `.agent-memory/ARCHITECTURE-DECISIONS.md`.
5. Read `docs/00-index.md` (if it exists).
6. Read only the specific documentation files relevant to the assigned task.
7. Consult `.agent-memory/CODEBASE-MAP.md` when navigating code structures.

*Note*: If a memory file does not yet exist, do not treat it as an error; it will be created during the dedicated memory initialization phase.  
*Rule*: Never assume prior CLI conversation history is accessible. The persistent memory files and approved documentation are the definitive sources of truth.

## 3.2. Source Code Inspection Policy

To minimize token consumption, context contamination, and redundant disk I/O, adhere to the following inspection rules:

- **No Blanket Scanning**: Do not inspect the entire solution or recursively traverse all directories by default.
- **No Unrelated Code Loading**: Do not open files outside the explicit scope of the current task.
- **Map-Driven Navigation**: Use `.agent-memory/CODEBASE-MAP.md` to pinpoint specific projects, classes, or interfaces before reading them.
- **Minimal Depth**: Read only the targeted source files and immediate dependencies required for task execution.
- **Discrepancy Resolution**: If verified source code and documentation conflict:
  1. Inspect the specific code section in question.
  2. Identify actual runtime behavior.
  3. Update and correct the documentation accordingly.
  4. Log the resolution in `.agent-memory/DEVELOPMENT-LOG.md`.
- **Integrity Mandate**: Never state or imply that code has been inspected, compiled, or tested if the operation was not actually executed.

---

# 4. DOCUMENTATION STANDARDS

All project documentation must reside within the `docs/` hierarchy (except root instructions and `.agent-memory/`).

### Documentation Quality Rules
- **Structure**: Clear, logical hierarchy with descriptive headings, tables, and standard diagrams.
- **Conciseness**: Technically precise, practical, and devoid of filler or redundant prose.
- **Language**: English is mandatory for technical specifications, architecture records, and code comments (unless the user explicitly requests Vietnamese for end-user guides).
- **Incremental Maintenance**: Update existing documentation in place rather than generating overlapping or contradictory copies.
- **No Hallucinations**: Never fabricate APIs, schemas, completed tasks, or requirements. When information is unknown or unverified, label it explicitly as `OPEN QUESTION` or `NOT VERIFIED`.

### Decision Classification
Every architectural or business decision documented must carry one of three explicit states:
- `CONFIRMED`: Explicitly approved by the user.
- `PROPOSED`: Recommended by the Agent, pending user review.
- `OPEN QUESTION`: Under evaluation; requires clarification or user input.

*Rule*: A `PROPOSED` decision must never be treated as `CONFIRMED` without explicit authorization from the user.

---

# 5. PROJECT MEMORY MAINTENANCE

At the conclusion of each completed task, update **only** the memory files affected by the work. Do not rewrite unrelated files or duplicate entries.

- **`PROJECT-STATUS.md`**: Update current project stage, milestone progress, newly approved decisions, open questions, and known project risks.
- **`ARCHITECTURE-DECISIONS.md`**: Record significant decisions with: Decision ID, Title, Status (`PROPOSED`/`CONFIRMED`), Context, Decision, Reasoning, Consequences, and Date.
- **`CODEBASE-MAP.md`**: Update the structural index when projects, major modules, entry points, DbContexts, core interfaces, or contracts are added or refactored.
- **`DEVELOPMENT-LOG.md`**: Append a concise chronological entry: Date, Task Identifier, Summary of Changes, Files Affected, Build/Test Results, and Outstanding Issues.
- **`NEXT-TASK.md`**: Define exactly one recommended next task containing: Objective, Scope, Required Inputs, Relevant Documentation, Expected Deliverables, Acceptance Criteria, and Approval Requirements.
- **`KNOWN-ISSUES.md`**: Track unresolved defects, technical debt, performance bottlenecks, and active workarounds.

---

# 6. DEVELOPMENT PRINCIPLES

When development phases commence, all implementation must rigorously adhere to:

- **Object-Oriented & Architectural Standards**: SOLID principles, Clean Code conventions, appropriate Domain-Driven Design (DDD) patterns, and strict Separation of Concerns.
- **Dependency Inversion**: High-level modules must depend on abstractions, registered via ASP.NET Core Dependency Injection.
- **Input Validation**: Enforce strict server-side validation (FluentValidation or standard data annotations) before input reaches domain services.
- **Robust Exception Handling**: Use structured domain exceptions and centralized middleware; never catch and silently swallow general exceptions.
- **Data Integrity**: Wrap multi-entity mutations (order placement, billing, inventory release) in explicit database transactions (`IDbContextTransaction`).
- **Concurrency Management**: Employ Optimistic Concurrency Control (`RowVersion` / Concurrency Stamp) for tables, orders, and payment records.
- **Idempotency**: Guarantee idempotent handling for order creation, bill calculation, and payment callbacks.
- **Testing**: Accompany business logic with unit, integration, and contract tests (xUnit, FluentAssertions, Moq).

### Practices to Avoid
- Avoid unnecessary abstractions or generic repositories that merely wrap EF Core.
- Avoid premature micro-optimizations that harm readability.
- Avoid duplicating business logic across controllers, services, or frontend scripts.
- Avoid introducing third-party packages without documenting purpose and trade-offs.

---

# 7. BUSINESS AND ARCHITECTURAL GOVERNANCE

SmartRestaurant is delivered strictly through controlled lifecycle stages:

1. **Stage 1**: Product Requirements & Business Domain Analysis.
2. **Stage 2**: System Architecture & Technical Design.
3. **Stage 3**: Detailed Module Specifications & Data Models.
4. **Stage 4**: Implementation & Milestone Planning.
5. **Stage 5**: Iterative Implementation by Approved Milestone.
6. **Stage 6**: Quality Verification & End-to-End Testing.
7. **Stage 7**: Deployment Preparation, Configuration & Pilot.

### Governance Rules
- Each stage must have explicit deliverables and acceptance criteria.
- **No Automatic Progression**: Never advance to a subsequent stage without explicit user review and confirmation.
- **No Premature Coding**: During Stages 1 through 4 (Analysis & Design), do not create business source code, dummy classes, or empty project scaffolding.
- **Scope Discipline**: During Stage 5 (Implementation), implement only the currently approved milestone. Do not begin work on future milestones prematurely.

---

# 8. FILE AND GIT SAFETY

Before creating, modifying, or deleting files, adhere to the following safety protocols:

1. **Inspect Before Edit**: Read the current file content and understand its dependencies before making changes.
2. **Minimal Invasiveness**: Make the smallest targeted change necessary to fulfill the objective.
3. **Prohibited Actions**:
   - Never delete unrelated files or directories.
   - Never overwrite existing configuration files without prior inspection.
   - Never touch or expose secret keys, API tokens, or production credentials.
   - Never execute destructive database operations (`DROP DATABASE`, data wiping) without explicit written confirmation.
   - Never execute destructive Git commands (`git reset --hard`, `git push --force`, `git clean -fdx`) without explicit permission.
   - Never commit (`git commit`) or push (`git push`) to version control unless the user explicitly instructs to do so.
4. **Scope Escalation**: If completing a task requires modifying files outside the agreed scope, halt, document the reason, and request user approval first.

---

# 9. BUILD AND TEST REQUIREMENTS

During implementation tasks, verification must be grounded in real command execution:

- **Compile Verification**: Execute `dotnet build` on affected projects and verify zero compilation errors.
- **Automated Testing**: Run relevant test suites (`dotnet test`) and inspect test results.
- **Honest Verification**: If a test or command cannot be run due to missing environment tools, LocalDB service unavailability, or missing secrets, clearly document the limitation. Never fabricate or assume passing test results.
- **Regression Awareness**: Prefer targeted unit tests for focused changes, but run broader module tests when modifications impact shared contracts or core entities.

---

# 10. SESSION CONTINUITY

Every AI Agent session must be treated as independent, stateless, and self-sufficient.

Any incoming Agent session must be capable of immediately understanding project status and resuming work using:
- `GEMINI.md`
- The `.agent-memory/` directory
- Approved documentation under `docs/`
- Verified source code

At the conclusion of every task, leave clear, unambiguous handover documentation in `.agent-memory/NEXT-TASK.md` and `.agent-memory/DEVELOPMENT-LOG.md` so that work continues seamlessly without requiring the user to re-explain project context.

---

# 11. TASK COMPLETION REPORT

At the conclusion of **every** task, provide a structured completion report covering these nine points:

1. **Task Summary**: Concise summary of what was requested and accomplished.
2. **Completed Work**: Bulleted list of concrete deliverables achieved.
3. **Files Created, Modified, or Deleted**: Complete manifest with exact repository paths.
4. **Decisions Made**: Architectural or operational choices finalized during the task.
5. **Decisions Requiring Approval**: Items proposed or pending user confirmation.
6. **Build and Test Results**: Actual outputs from compilation and test execution.
7. **Known Issues and Risks**: Any newly identified defects, blockers, or architectural risks.
8. **Project Memory Files Updated**: Specific `.agent-memory/` files modified.
9. **Recommended Next Task**: Clear, single next step aligned with `.agent-memory/NEXT-TASK.md`.
