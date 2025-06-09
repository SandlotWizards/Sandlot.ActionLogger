# Sandlot.ActionLogger

A structured, hierarchical, and developer-friendly logging utility for tracking progress in CLI tools, deployment scripts, background jobs, and long-running operations. Built to enhance developer experience, visibility, and performance diagnostics.

---

## 📘 Purpose

`Sandlot.ActionLogger` provides a reusable logging abstraction called `ActionLoggerService`, enabling:

- Visually structured CLI feedback (`✔`, `❌`, `⚠️`, `⟳`)
- Elapsed time measurement and threshold warnings
- Hierarchical nesting for sub-steps
- Seamless integration with `ILogger`
- Developer-friendly fluent syntax

It is also the engine behind the `sandlot-copilot action-logger` CLI verb, used as a diagnostic and training harness.

---

## ✨ Features

- ✅ **Structured Output**: Steps rendered in tree-like form (e.g., `1.`, `1.1.`, `1.1.1.`)
- ✅ **Visual CLI Feedback**: Rich symbols and color-coded indicators
- ✅ **Time Tracking**: Built-in elapsed duration for each step
- ✅ **Soft Threshold Alerts**: Passive warnings when durations exceed limits
- ✅ **Optional Telemetry**: Emits logs through `ILogger`
- ✅ **Developer-First Syntax**: `using (_tracker.BeginStep(...)) { ... }`

---

## 🚀 Example Usage

```csharp
using (_tracker.BeginStep("Initialize environment"))
{
    _tracker.Info("Connecting to database...");
    ConnectToDb();

    _tracker.Info("Seeding initial data...");
    SeedData();

    _tracker.Success("Initialization complete");
}
