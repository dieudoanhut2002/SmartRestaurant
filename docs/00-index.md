# SmartRestaurant — Documentation Index

Welcome to the centralized documentation index for the **SmartRestaurant** project. All formal technical specifications, requirements, architecture designs, and runbooks reside within this hierarchy.

---

## 1. Governance & Project Instructions

| Document | Description | Scope |
| :--- | :--- | :--- |
| **[GEMINI.md](../GEMINI.md)** | Authoritative project rules, engineering governance, memory strategy, and safety protocols. | Root Project Governance |
| **[AGENTS.md](../AGENTS.md)** | Engineering standards, clean architecture rules, coding conventions, and output mandates. | Engineering & Quality Rules |

---

## 2. Product Discovery & Business Specifications (Stage 1)

### 2.1. Master Product Specifications
| Document | Title | Status | Description |
| :--- | :--- | :--- | :--- |
| **[docs/product/PRD.md](./product/PRD.md)** | Product Requirements Document (PRD) | `CONFIRMED` | Product vision, target personas, business objectives, KPIs, and scope. |
| **[docs/product/PRODUCT-REVIEW-REPORT.md](./product/PRODUCT-REVIEW-REPORT.md)** | Báo Cáo Đánh Giá & Thẩm Định Nghiệp Vụ Chuyên Sâu | `CONFIRMED` | Phản biện độc lập, 4 vấn đề CRITICAL, 6 vấn đề HIGH, rà soát 12 kịch bản vận hành thực tế. |
| **[docs/product/PRODUCT-REVIEW-VALIDATION.md](./product/PRODUCT-REVIEW-VALIDATION.md)** | Product Review & Business Validation Report | `CONFIRMED` | Comprehensive cross-document consistency audit, persona stress tests, and Stage 2 readiness. |
| **[docs/product/VIETNAMESE-LOCALIZATION-STANDARD.md](./product/VIETNAMESE-LOCALIZATION-STANDARD.md)** | Vietnamese-First Localization Standard | `CONFIRMED` | Complete terminology dictionary, regional formats (VND, dates), printing, and UI standards. |
| **[docs/product/BUSINESS-REQUIREMENTS.md](./product/BUSINESS-REQUIREMENTS.md)** | Business Requirements Specification | `CONFIRMED` | Granular functional requirements, actors, business rules, and exceptions. |
| **[docs/product/MVP-SCOPE.md](./product/MVP-SCOPE.md)** | MVP Scope & Release Roadmap | `CONFIRMED` | MoSCoW feature matrix, explicit exclusions, and release milestones. |
| **[docs/product/DOMAIN-GLOSSARY.md](./product/DOMAIN-GLOSSARY.md)** | Ubiquitous Domain Glossary | `CONFIRMED` | Standardized English & Vietnamese operational vocabulary. |
| **[docs/product/USER-JOURNEYS.md](./product/USER-JOURNEYS.md)** | Comprehensive User Journeys | `CONFIRMED` | 10 end-to-end actor journeys covering daily restaurant routines. |
| **[docs/product/OPEN-QUESTIONS.md](./product/OPEN-QUESTIONS.md)** | Open Business Questions & Decision Log | `OPEN` | Tier 1 (MVP Blockers) and Tier 2 (Post-MVP) decision analysis. |
| **[docs/product/MODULE-OVERVIEW.md](./product/MODULE-OVERVIEW.md)** | Business Module Overview & Boundaries | `CONFIRMED` | 14 core business module boundaries, entities, and dependencies. |
| **[docs/product/NON-FUNCTIONAL-REQUIREMENTS.md](./product/NON-FUNCTIONAL-REQUIREMENTS.md)** | Product Non-Functional Requirements | `CONFIRMED` | Security, data isolation, performance targets, and concurrency rules. |

### 2.2. Critical Operational Workflows
| Document | Title | Status | Scope |
| :--- | :--- | :--- | :--- |
| **[docs/product/workflows/FLOOR-PLAN-WORKFLOW.md](./product/workflows/FLOOR-PLAN-WORKFLOW.md)** | Floor Plan & Table Management Workflow | `CONFIRMED` | Table states, visual canvas, table move, merge, unmerge, and VIP room booking. |
| **[docs/product/workflows/ORDER-WORKFLOW.md](./product/workflows/ORDER-WORKFLOW.md)** | POS & Order Management Workflow | `CONFIRMED` | Touch order capture, modifiers, order rounds, firing, remake dish, and void overrides. |
| **[docs/product/workflows/KITCHEN-WORKFLOW.md](./product/workflows/KITCHEN-WORKFLOW.md)** | Kitchen Routing & KDS Operational Workflow | `CONFIRMED` | Multi-station routing, KDS cards, SLA timers, remake tickets, and expediter pacing. |
| **[docs/product/workflows/CASHIER-PAYMENT-WORKFLOW.md](./product/workflows/CASHIER-PAYMENT-WORKFLOW.md)** | Cashier, Billing & Payment Workflow | `CONFIRMED` | Multi-bill split without table freeze, VietQR delayed suspense reconciliation, and shift Z-Reports. |

### 2.3. Product Decision Records (PDRs)
| Decision ID | Document | Status | Summary |
| :--- | :--- | :--- | :--- |
| **PDR-001** | [docs/product/decisions/PDR-001-modular-monolith-deployment-model.md](./product/decisions/PDR-001-modular-monolith-deployment-model.md) | `CONFIRMED` | Modular Monolith deployment with early multi-tenant/branch data isolation. |
| **PDR-002** | [docs/product/decisions/PDR-002-hierarchical-area-room-table-model.md](./product/decisions/PDR-002-hierarchical-area-room-table-model.md) | `CONFIRMED` | Hierarchical Area $\rightarrow$ Room $\rightarrow$ Table spatial layout model. |
| **PDR-003** | [docs/product/decisions/PDR-003-independent-station-kitchen-routing.md](./product/decisions/PDR-003-independent-station-kitchen-routing.md) | `CONFIRMED` | Dynamic station routing with Master Expediter display at kitchen pass. |
| **PDR-004** | [docs/product/decisions/PDR-004-inventory-bom-deduction-trigger-policy.md](./product/decisions/PDR-004-inventory-bom-deduction-trigger-policy.md) | `CONFIRMED` | Two-phase inventory lock (`Available = Physical - Reserved`) & Countable items in MVP. |
| **PDR-005** | [docs/product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md](./product/decisions/PDR-005-multi-tender-and-bill-splitting-strategy.md) | `CONFIRMED` | Multi-tender checkout with Split by Items and Split Evenly ($1/N$). |
| **PDR-006** | [docs/product/decisions/PDR-006-vietnamese-language-ui.md](./product/decisions/PDR-006-vietnamese-language-ui.md) | `CONFIRMED` | 100% Vietnamese language presentation layer and user interfaces. |
| **PDR-007** | [docs/product/decisions/PDR-007-business-date-shift-management.md](./product/decisions/PDR-007-business-date-shift-management.md) | `CONFIRMED` | Configurable Business Date (Ngày Kinh Doanh) and Cutoff Time for overnight shifts. |
| **PDR-008** | [docs/product/decisions/PDR-008-vietqr-delayed-webhook-handling.md](./product/decisions/PDR-008-vietqr-delayed-webhook-handling.md) | `CONFIRMED` | Resolution policy for delayed VietQR webhooks, suspense ledger, and manual audit. |
| **PDR-009** | [docs/product/decisions/PDR-009-order-freeze-during-bill-split.md](./product/decisions/PDR-009-order-freeze-during-bill-split.md) | `CONFIRMED` | Multi-Bill per Table Session and flexible split billing policy without table locking. |
| **PDR-010** | [docs/product/decisions/PDR-010-unmerge-tables.md](./product/decisions/PDR-010-unmerge-tables.md) | `CONFIRMED` | Table unmerge and line item reallocation policy preserving kitchen ticket integrity. |
| **PDR-011** | [docs/product/decisions/PDR-011-area-and-vip-room-surcharges.md](./product/decisions/PDR-011-area-and-vip-room-surcharges.md) | `CONFIRMED` | Percentage and fixed fee surcharges for dining areas and private VIP rooms. |
| **PDR-012** | [docs/product/decisions/PDR-012-direct-service-items-foh.md](./product/decisions/PDR-012-direct-service-items-foh.md) | `CONFIRMED` | Direct front-of-house service routing bypassing Bar KDS preparation queue. |
| **PDR-013** | [docs/product/decisions/PDR-013-shared-room-and-entire-room-booking.md](./product/decisions/PDR-013-shared-room-and-entire-room-booking.md) | `CONFIRMED` | Independent table lifecycle within shared rooms and entire room reservation locks. |
| **PDR-014** | [docs/product/decisions/PDR-014-concurrent-order-append-business-rules.md](./product/decisions/PDR-014-concurrent-order-append-business-rules.md) | `CONFIRMED` | Business rules for concurrent waiter ordering without table locking (Tech: `PROPOSED`). |
| **PDR-015** | [docs/product/decisions/PDR-015-remake-dish-workflow.md](./product/decisions/PDR-015-remake-dish-workflow.md) | `CONFIRMED` | Remake dish workflow for kitchen errors with bill protection and waste logging. |

---

## 3. Architecture & Technical Design (Stage 2 — Upcoming)

| Document | Title | Status | Target Stage |
| :--- | :--- | :--- | :--- |
| *docs/02-architecture/system-architecture.md* | Modular Monolith System Architecture & Component Design | `PLANNED` | Stage 2 |
| *docs/02-architecture/data-models-and-erd.md* | Entity-Relationship & Relational Schema Design | `PLANNED` | Stage 2 |
| *docs/02-architecture/realtime-signalr-mesh.md* | SignalR Real-Time Hubs & Event Contracts Specification | `PLANNED` | Stage 2 |

---

## 4. Decision Taxonomy
- `CONFIRMED`: Explicitly approved by project owner.
- `PROPOSED`: Recommended by the Product Architect, pending sign-off.
- `OPEN QUESTION`: Actively being evaluated or requiring clarification.
