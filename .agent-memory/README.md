# SmartRestaurant — Project Memory System

This directory (`.agent-memory/`) contains persistent, version-controlled project context for all AI Agent pair-programming sessions and engineering leads working on **SmartRestaurant**.

## Structure & Purpose of Memory Files

| File | Primary Purpose | Update Frequency |
| :--- | :--- | :--- |
| **`PROJECT-STATUS.md`** | High-level stage tracking, milestones, active risks, and open questions. | Every task |
| **`ARCHITECTURE-DECISIONS.md`** | Formal Architectural Decision Records (ADRs) with rationale and consequences. | When significant decisions are made/proposed |
| **`CODEBASE-MAP.md`** | Navigational catalog of projects, folders, entry points, contracts, and DbContexts. | Incremental as code changes |
| **`DEVELOPMENT-LOG.md`** | Chronological audit trail of executed tasks, build/test results, and changes. | Every task |
| **`NEXT-TASK.md`** | Authoritative specification for the single approved next task. | Every task |
| **`KNOWN-ISSUES.md`** | Technical debt, unresolved bugs, and operational caveats. | When issues arise or are resolved |

## Rules of Engagement
- Memory files must be kept concise, factual, and free of conversational fluff.
- All technical documentation must live in `docs/` and be indexed in `docs/00-index.md`.
- See root [GEMINI.md](../GEMINI.md) for permanent rules, governance stages, and safety constraints.
