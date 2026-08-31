# SchemaDriftDetector

A lightweight .NET library that automatically detects breaking changes in your API responses — before your frontend does.

## The Problem

In any system with separate backend and frontend teams, the API response shape changes over time: a field gets removed, a data type changes, a required field silently becomes optional. These changes happen without real coordination, and nobody notices until the frontend crashes or QA stumbles onto a bug.

Real-world evidence: an audit of a mid-size SaaS company with 47 internal endpoints found that 23 had at least one undocumented structural change over 6 months, and 9 had type changes — the most dangerous kind of drift. None of it triggered a single failing unit test, because the *behavior* was still correct — only the *shape* had changed.

## What It Does

SchemaDriftDetector plugs into any ASP.NET Core project with a single line of code and:

- **Automatically learns** the real runtime schema of every endpoint's response — no OpenAPI spec required
- **Detects breaking changes** the moment they happen, by comparing each response against a stored baseline
- **Filters out noise**: naturally optional fields, role-based response differences, empty arrays, and one-off request blips are not mistaken for real drift
- **Debounces** — the same change must repeat at least twice consecutively before an alert fires, so a single blip never triggers false noise
- **Notifies instantly** via Slack or Discord, batched to avoid spam
- **Exposes a status endpoint** (`GET /api/drift/status`) for on-demand inspection of every tracked endpoint

## Why Not Just Use Pact / OpenAPI Validators?

| | Existing tools (Pact, etc.) | SchemaDriftDetector |
|---|---|---|
| Requires both teams to adopt a framework | Yes | No — backend-only |
| Requires an accurate pre-existing spec | Yes | No — learns from real traffic |
| Catches runtime surprises (unexpected null, etc.) | Usually not | Yes — this is the core idea |
| Setup effort | Heavy, needs cross-team coordination | One middleware line |

## How It Works

```
Incoming Request
      │
      ▼
SchemaFingerprint Middleware   → intercepts the response (2xx only)
      │
      ▼
Schema Extractor               → converts JSON into a tree structure (SchemaNode)
      │
      ▼
Drift Detector / Diffing Engine → compares against baseline + debounce logic
      │
      ├──► SQLite (Baseline + History)
      │
      └──► Drift Batcher → Notifier → Slack / Discord
```

Every JSON response is converted into a tree structure (`SchemaNode`), where each node tracks its data type, nullable/optional flags, and children — comparison is a recursive tree diff, the same principle compilers use to diff two ASTs and React's Virtual DOM diffing uses to find minimal changes between two trees.

The library is a **class library**, not a standalone API — it's referenced and injected into an existing ASP.NET Core project the same way you'd use Serilog or MediatR. The only exception is a single Minimal API endpoint (`GET /api/drift/status`) it registers into the host app.

Internally the project uses a **simple layered folder structure**, not full Clean Architecture — a deliberate choice, since this is an infrastructure/observability tool with no complex business rules to isolate.

## Project Structure & Implementation Status

```
SchemaDriftDetector/
├── Core/            ✅ Done  → SchemaNode, SchemaExtractor, SchemaDiffer, SchemaMerger,
│                                SchemaDifference, SeverityRules (tree diffing, no external deps)
├── Storage/         ✅ Done  → EF Core DbContext, 6 entities (Endpoint, SchemaBaseline,
│                                SchemaVersion, PendingDrift, DriftAlert, Deploy), SQLite,
│                                first migration generated and applied
├── Detection/       🔨 In progress → orchestrator: endpoint key resolution (route template +
│                                environment + role), new/drift/pending/confirmed decision logic,
│                                debounce state machine driven by SchemaNode.StructurallyEquals
├── Notifications/   📋 Planned → Slack/Discord webhook sender, Polly retry, time-window batching
└── Middleware/      📋 Planned → SchemaFingerprintMiddleware + UseSchemaDriftDetection()
```

`ServiceCollectionExtensions.AddSchemaDriftDetection(...)` (project root) is already implemented and wires up `Storage/` today; it will grow to register `Notifications/` and `Middleware/` as those layers land.

## Getting Started

> The package isn't published to NuGet yet — the snippet below shows the target developer experience once `Middleware/` is complete.

```bash
dotnet add package SchemaDriftDetector
```

```csharp
// Program.cs
builder.Services.AddSchemaDriftDetection(options =>
{
    options.SlackWebhookUrl = builder.Configuration["DriftDetector:SlackWebhook"];
    options.DebounceThreshold = 2;
});

var app = builder.Build();
app.UseSchemaDriftDetection();
```

That's it — no OpenAPI spec, no consumer contracts, no coordination with the frontend team required to get started.

## Computer Science Foundations

- **Tree diffing** — recursive structural comparison of JSON-derived schema trees (AST-diff / Virtual DOM style)
- **Automatic SemVer enforcement** — field removed or type changed → Major; new optional field → Minor; no shape change → Patch
- **State machine** — each `PendingDrift` moves through `Observing → Confirmed → Discarded`, not a plain boolean
- **Debounce** — a proposed change must repeat identically (via `StructurallyEquals`) before it's confirmed
- **Producer-consumer via bounded channel** (`System.Threading.Channels`) — the middleware pushes the extracted fingerprint and returns immediately; a background worker drains it, so drift detection never adds latency to the request path
- **Per-endpoint locking** — prevents lost updates / duplicate alerts under concurrent requests to the same endpoint, without serializing unrelated endpoints
- **Deploy-aware baselining** — schema comparison is tied to a deployment identifier (commit hash/build ID) rather than compared request-by-request, so a shared Staging environment doesn't cause baseline flapping between concurrent deploys

## Tech Stack

- **.NET 8** — ASP.NET Core Middleware
- **System.Text.Json** — schema extraction
- **SQLite + EF Core** — baseline and history storage
- **Polly** — retry with exponential backoff (installed, wired into `Notifications/` next)
- **Slack / Discord Webhooks** — notifications

## Status

🚧 Work in progress.

- ✅ `Core/` — tree representation, extraction, and diffing engine fully implemented
- ✅ `Storage/` — all 6 entities, `DbContext`, design-time factory, and first EF Core migration applied
- 🔨 `Detection/` — orchestrator layer, currently being built
- 📋 `Notifications/` and `Middleware/` — planned next

## Practical Value

- Grounded in real computer science foundations: tree diffing, graph theory, state machines, distributed-systems reliability patterns
- Addresses a genuine, under-served gap: existing tools (Pact) are powerful but heavy to set up; this is zero-config from the backend side alone
- Intended to be validated end-to-end on real personal projects (Rujta, Routine) once packaged
- Extensible: starts as a simple NuGet package, can grow into a full SaaS product later (dashboard, multi-project support, VS Code extension)
