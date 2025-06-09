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

It is also the engine behind the `sandlot-copilot action-logger` CLI verb, used as a diagnostic and training harness. ** Coming Soon

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
```

With threshold warning:
```csharp
using (_tracker.BeginStep("Fetch remote config", TimeSpan.FromMilliseconds(500)))
{
    FetchRemoteConfig();
}
```

CLI output:
```
1. Initialize environment
  ⟳ Connecting to database...
  ⟳ Seeding initial data...
  ✔ Initialization complete (245ms)
2. Fetch remote config
  ⚠️ ✔ Fetch remote config (824ms) — exceeded threshold
```

---

## 🔌 Interface

```csharp
public interface IActionLoggerService
{
    IDisposable BeginStep(string title, TimeSpan? threshold = null);
    string Info(string message, bool logToLogger = false);
    string Success(string message = "✔ Done", bool logToLogger = false);
    string Error(string message, bool logToLogger = true);
}
```

---

## 🧪 CLI Support – `sandlot-copilot action-logger`

This project powers the `action-logger` verb in the `sandlot-copilot` CLI:

```bash
sandlot-copilot action-logger --use-case U01
sandlot-copilot action-logger --group struct
sandlot-copilot action-logger --demo-example
```

- 15+ diagnostic use cases (`U01–U15`)
- Structured test groups (`struct`, `ttl`, `sev`, `devx`)
- Real-time CLI demonstrations

---

## 🧱 Integration

To use this in your DI container:

```csharp
services.AddScoped<IActionLoggerService, ActionLoggerService>();
```

Recommended for scoped lifetimes during CLI commands or background jobs.

Namespaces:
- Interface: `Sandlot.ActionLogger.Interfaces`
- Implementation: `Sandlot.ActionLogger.Services`

---

## 📦 Projects

- `Sandlot.ActionLogger` — Core implementation
- `Sandlot.ActionLogger.Tests` — Unit tests for service behavior

---

## 🛠 Status

✅ **Production-ready**  
All CLI tooling and operational scripts within an organization are encouraged to standardize on this logging pattern.

---

## 📄 License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

## 🙌 Contributing

We welcome contributions! Submit issues or pull requests for improvements, bug fixes, or new CLI diagnostics.

---

## 🧭 Authoritative Documentation

All details, patterns, and CLI behavior for `ActionLoggerService` and its usage are documented in the file [`ActionLoggerService.md`](./ActionLoggerService.md).
