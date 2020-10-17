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

| Serilog concept | Unified Logging concept |
|-----------------|-------------------------|
| SourceContext   | Category                |

One common pattern of how you might use this: first, make an instance (or static) variable `Log` in your class:

```csharp
public class MyMusicPlayer
{
    readonly ILogger Log = Serilog.Log.ForContext<MyMusicPlayer>();
}
```

Notice how we're passing the class name as a generic type to `ForContext<T>`.

Second, call a method on `Log`:

```csharp
public void Play()
{
    Playback.Start();

    Log.Debug("PlayerState is now {this.PlayerState}");
}
```

The log message will implicitly have the special property **SourceContext** set to `MyMusicPlayer`.

Now, when you launch Console, you can:

* show the **Category** column
* filter by it, such as by typing `category:MyMusicPlayer` in the search field and hitting return.

### Subsystems

Currently, Unified Logging's **Subsystem** will map to the bundle ID of the current process.

## Potential improvements

This is just a fairly simple, straightforward implementation so far.

Both Serilog and Unified Logging offer additional features that this won't cover just yet, such as structured logging (this will just output plain text, for the most part) and signposts.
