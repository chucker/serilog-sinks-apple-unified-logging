# serilog-sinks-apple-unified-logging
A Serilog sink for Apple Unified Logging.

## Intro

**What's Serilog?**

[Serilog](https://serilog.net) is a .NET logging framework that supports pluggable "sinks", i.e. targets where the log output goes (a console, a file, a database, etc.). Applications use it to write log messages, and sinks then decide where they go.

**What's Apple Unified Logging?**

[Apple Unified Logging](https://developer.apple.com/documentation/os/logging) is an system-level logging facility for Apple's various modern OSes, most prominently iOS and macOS.

macOS ships with an app called *Console* to view such logs. One of cool thing about that app is it'll wirelessly receive logs from other devices, such as your iPhone or Apple Watch, and just display them as additional sources in the sidebar.

**How does this project help me?**

If you're writing a [Xamarin](https://dotnet.microsoft.com/apps/xamarin) app (including Xamarin Forms), and want your logs to end up in Apple's recommended destination, this project will make that rather easy.

## Concepts

This project attempts to map Serilog's various concepts such as log event levels to Apple's concepts. This doesn't always work 1:1.

### Log event levels

| Serilog log level | Unified Logging log level |
|-------------------|---------------------------|
| Verbose           | Debug                     |
| Debug             | Debug                     |
| Information       | Info                      |
| Warning           | Error                     |
| Error             | Error                     |
| Fatal             | Fault                     |

### Source contexts / categories


