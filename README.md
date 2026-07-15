# SchemaDriftDetector

A lightweight .NET library that automatically detects breaking changes in your API responses — before your frontend does.

## The Problem

In any system with separate backend and frontend teams, the API response shape changes over time: a field gets removed, a data type changes, a required field silently becomes optional. These changes happen without real coordination, and nobody notices until the frontend crashes or QA stumbles onto a bug.

## What It Does

SchemaDriftDetector plugs into any ASP.NET Core project with a single line of code and:

- **Automatically learns** the real runtime schema of every endpoint's response — no OpenAPI spec required
- **Detects breaking changes** the moment they happen, by comparing each response against a stored baseline
- **Filters out noise**: naturally optional fields, role-based response differences, empty arrays, and one-off request blips are not mistaken for real drift
- **Notifies instantly** via Slack or Discord, batched and debounced to avoid spam
- **Exposes a status endpoint** (`GET /api/drift/status`) for on-demand inspection of every tracked endpoint

## Why Not Just Use Pact / OpenAPI Validators?

| | Existing tools (Pact, etc.) | SchemaDriftDetector |
|---|---|---|
| Requires both teams to adopt a framework | Yes | No — backend-only |
| Requires an accurate pre-existing spec | Yes | No — learns from real traffic |
| Catches runtime surprises (unexpected null, etc.) | Usually not | Yes — this is the core idea |
| Setup effort | Heavy | One middleware line |

## How It Works

```
Incoming Request
      │
      ▼
SchemaFingerprint Middleware   → intercepts the response (2xx only)
      │
      ▼
Schema Extractor               → converts JSON into a tree structure
      │
      ▼
Drift Detector / Diffing Engine → compares against baseline + debounce logic
      │
      ├──► SQLite (Baseline + History)
      │
      └──► Drift Batcher → Notifier → Slack / Discord
```

The library is a **class library**, not a standalone API — it's referenced and injected into an existing ASP.NET Core project the same way you'd use Serilog or MediatR.

## Project Structure

```
SchemaDriftDetector/
├── Core/            → Tree representation, JSON extraction, diffing (no external dependencies)
├── Storage/          → EF Core DbContext, entities, repository (SQLite)
├── Detection/         → Orchestration, debounce logic, endpoint key resolution
├── Notifications/      → Slack/Discord webhook, retry, batching
└── Middleware/        → Request pipeline integration, DI registration
```

## Getting Started

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

## Tech Stack

- **.NET 8** — ASP.NET Core Middleware
- **System.Text.Json** — schema extraction
- **SQLite + EF Core** — baseline and history storage
- **Polly** — retry with exponential backoff
- **Slack / Discord Webhooks** — notifications

## Status

🚧 Work in progress — core diffing engine (`Core/`) implemented. Storage, Detection, Notifications, and Middleware layers in progress.

## License

MIT
