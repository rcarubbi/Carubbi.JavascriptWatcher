# Carubbi.JavascriptWatcher

.NET automation layer for WebView2 JavaScript dialogs. Declarative policies for `alert`, `confirm`, and `window.open`, with embedded-window routing and a multi-WebView manager.

[![NuGet](https://img.shields.io/nuget/v/Carubbi.JavascriptWatcher.svg)](https://www.nuget.org/packages/Carubbi.JavascriptWatcher)
[![NuGet](https://img.shields.io/nuget/dt/Carubbi.JavascriptWatcher.svg)](https://www.nuget.org/packages/Carubbi.JavascriptWatcher)

## Requirements

- .NET 10 or later
- Microsoft Edge WebView2 Runtime installed
- Windows (the library uses WinForms)

## Installation

```
dotnet add package Carubbi.JavascriptWatcher
```

## Quick start

### Event-based (v2-compatible)

```csharp
using Carubbi.JavascriptWatcher;
using Microsoft.Web.WebView2.WinForms;

WebView2 webView = ...;

var watcher = new JavascriptWatcher(webView);

watcher.AlertIntercepted += (s, e) => Console.WriteLine($"Alert: {e.Message}");
watcher.ConfirmIntercepted += (s, e) =>
{
    Console.WriteLine($"Confirm: {e.Message}");
    e.Result = true; // accept the confirm
};
watcher.WindowOpenIntercepted += (s, e) => Console.WriteLine($"Window open: {e.Url}");

watcher.Start(suppressAlert: true, suppressWindowOpen: true);
```

### Declarative policy (v3)

Remove the `if` logic from your app by describing *what* to do per message/URL:

```csharp
var alertPolicy = new AlertPolicy();
alertPolicy.WhenMessageMatches("session expired").Suppress();
alertPolicy.WhenSourceUrlMatches(@"https://app\.example\.com").Show();

var confirmPolicy = new ConfirmPolicy();
confirmPolicy.WhenMessageMatches(@"delete").AutoAccept();
confirmPolicy.WhenSourceUrlMatches(@"evil\.com").AutoReject();

var windowPolicy = new WindowOpenPolicy();
windowPolicy.WhenUrlMatches(@"docs\.example\.com").RouteTo(new WindowOpenTarget(docsPane));
windowPolicy.WhenUrlMatches(@"ads\.example\.com").Suppress();
windowPolicy.WhenUrlMatches(@"oauth\.").OpenExternally();

var watcher = new JavascriptWatcher(webView, alertPolicy, confirmPolicy, windowPolicy);
watcher.Start();
```

### Embedded routing for `window.open`

Instead of opening an external popup, route the requested content into a docked WebView2 pane:

```csharp
WindowOpenTarget docsPane = new(dockPane); // dockPane is a WebView2 control

var windowPolicy = new WindowOpenPolicy();
windowPolicy.WhenUrlMatches(@"^https://docs\.example\.com").RouteTo(docsPane);

var watcher = new JavascriptWatcher(webView, null, null, windowPolicy);
watcher.Start();
```

### Multi-WebView manager

One central policy applied to many controls:

```csharp
var manager = new JavascriptWatcherManager(alertPolicy, confirmPolicy, windowPolicy);

manager.Add(webViewMain);
manager.Add(webViewAux);
// ...
manager.StopAll();
```

### Auditing / telemetry

Every dialog resolution is recorded through an `IDialogAuditSink`:

```csharp
public sealed class TraceAuditSink : IDialogAuditSink
{
    public void Record(DialogResolvedEventArgs e) =>
        System.Diagnostics.Trace.WriteLine(
            $"[{e.Timestamp:O}] {e.DialogType} -> {e.Decision?.Kind} | {e.Message} | {e.SourceUrl}");
}

var watcher = new JavascriptWatcher(webView, alertPolicy, confirmPolicy, windowPolicy, new TraceAuditSink());
watcher.Start();
```

## Policies

| Policy | Matcher | Decision |
|--------|---------|----------|
| `AlertPolicy` | `WhenMessageMatches`, `WhenSourceUrlMatches` | `Suppress()`, `Show()` |
| `ConfirmPolicy` | `WhenMessageMatches`, `WhenSourceUrlMatches` | `AutoAccept()`, `AutoReject()` |
| `WindowOpenPolicy` | `WhenUrlMatches` | `RouteTo(target)`, `Suppress()`, `OpenExternally()` |

Policies are evaluated in registration order via `IDialogPolicy.Evaluate(DialogContext)`. The first matching rule wins; otherwise the dialog passes through to the default browser behavior. Combine policies with `CompositePolicy`.

## How it works

Unlike the old version (which injected JavaScript to override `window.alert`, `window.confirm`, and `window.open` in the DOM via MSHTML), version 3.x uses **native WebView2 events**:

- `ScriptDialogOpening` — intercepts `alert`, `confirm`, `prompt`, and `beforeunload`
- `NewWindowRequested` — intercepts `window.open`

No JavaScript is injected into the page, and no COM (MSHTML) reference is required.

## Migrating from v1.x / v2.x

v3.0.0 is a **breaking** release from v1; the v2 event API (`Start`/`Stop` + `AlertIntercepted`/`ConfirmIntercepted`/`WindowOpenIntercepted`) is preserved and v3 adds policies, routing, the manager, and auditing.

| v1.x | v3.x |
|------|------|
| `new JavascriptWatcher(WebBrowser)` | `new JavascriptWatcher(WebView2)` |
| `AlertInterceptedEventArgs.AlertText` | `AlertInterceptedEventArgs.Message` |
| `ConfirmInterceptedEventArgs.Text` | `ConfirmInterceptedEventArgs.Message` |
| `ScriptInterface` (COM) | Removed — native events |
| `AttachHandlers()` | Wired automatically in `Start()` |
| jQuery 1.11.1 injection | Removed |
| `Start(bool, bool)` | Plus `Start()` with declarative policies |

## Development

### Build

```
dotnet build Carubbi.JavascriptWatcher.slnx -c Release
```

### Tests

```
dotnet run --project tests/Carubbi.JavascriptWatcher.Tests/Carubbi.JavascriptWatcher.Tests.csproj -c Release
```

### Coverage

```
powershell -File scripts/run-coverage.ps1 -Report -Open
```

Generates an HTML report at `TestResults/coverage/html/index.html`.

## CI/CD

- `.github/workflows/ci.yml` — build + tests on push/PR to `master`
- `.github/workflows/publish.yml` — pack + publish to NuGet.org when a `v*` tag is created

## License

MIT
