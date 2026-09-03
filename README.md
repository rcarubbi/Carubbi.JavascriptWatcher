# Carubbi.JavascriptWatcher

.NET library to track and handle JavaScript events (`alert`, `confirm`, `window.open`) via **Microsoft Edge WebView2**.

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

## Usage

```csharp
using Carubbi.JavascriptWatcher;
using Microsoft.Web.WebView2.WinForms;

// The WebView2 control from your WinForms app
WebView2 webView = ...;

var watcher = new JavascriptWatcher(webView);

watcher.AlertIntercepted += (s, e) => Console.WriteLine($"Alert: {e.Message}");
watcher.ConfirmIntercepted += (s, e) =>
{
    Console.WriteLine($"Confirm: {e.Message}");
    e.Result = true; // accept the confirm
};
watcher.WindowOpenIntercepted += (s, e) => Console.WriteLine($"Window open: {e.Url}");

// Suppress the default browser alert/window.open
watcher.Start(suppressAlert: true, suppressWindowOpen: true);
```

## How it works

Unlike the old version (which injected JavaScript to override `window.alert`, `window.confirm`, and `window.open` in the DOM via MSHTML), version 2.x uses **native WebView2 events**:

- `ScriptDialogOpening` — intercepts `alert`, `confirm`, `prompt`, and `beforeunload`
- `NewWindowRequested` — intercepts `window.open`

No JavaScript is injected into the page, and no COM (MSHTML) reference is required.

## Migrating from v1.x to v2.x

v2.0.0 is a **breaking** release:

| v1.x | v2.x |
|------|------|
| `new JavascriptWatcher(WebBrowser)` | `new JavascriptWatcher(WebView2)` |
| `AlertInterceptedEventArgs.AlertText` | `AlertInterceptedEventArgs.Message` |
| `ConfirmInterceptedEventArgs.Text` | `ConfirmInterceptedEventArgs.Message` |
| `ScriptInterface` (COM) | Removed — native events |
| `AttachHandlers()` | Wired automatically in `Start()` |
| jQuery 1.11.1 injection | Removed |

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
