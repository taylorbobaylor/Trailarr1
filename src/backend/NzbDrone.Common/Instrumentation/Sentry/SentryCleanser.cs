using System;
using System.Linq;
using Sentry;

namespace NzbDrone.Common.Instrumentation.Sentry;

public static class SentryCleanser
{
    public static SentryEvent CleanseEvent(SentryEvent sentryEvent)
    {
        ArgumentNullException.ThrowIfNull(sentryEvent);

        try
        {
            if (sentryEvent.Message is not null)
            {
                sentryEvent.Message.Formatted = CleanseLogMessage.Cleanse(sentryEvent.Message.Formatted ?? string.Empty);
                sentryEvent.Message.Message = CleanseLogMessage.Cleanse(sentryEvent.Message.Message ?? string.Empty);
                sentryEvent.Message.Params = sentryEvent.Message.Params?.Select(x => CleanseLogMessage.Cleanse(x switch
                {
                    string str => str,
                    _ => x?.ToString() ?? string.Empty
                })).ToList();
            }

            if (sentryEvent.Fingerprint.Any())
            {
                var fingerprint = sentryEvent.Fingerprint.Select(x => CleanseLogMessage.Cleanse(x)).ToList();
                sentryEvent.SetFingerprint(fingerprint);
            }

            if (sentryEvent.Extra.Any())
            {
                var extras = sentryEvent.Extra.ToDictionary(
                    x => x.Key,
                    y => (object)(y.Value as string != null ? CleanseLogMessage.Cleanse(y.Value as string) : y.Value ?? string.Empty));
                sentryEvent.SetExtras(extras);
            }

            if (sentryEvent.SentryExceptions is not null)
            {
                foreach (var exception in sentryEvent.SentryExceptions)
                {
                    exception.Value = CleanseLogMessage.Cleanse(exception.Value);
                    if (exception.Stacktrace is not null)
                    {
                        foreach (var frame in exception.Stacktrace.Frames)
                        {
                            frame.FileName = ShortenPath(frame.FileName);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
        }

        return sentryEvent;
    }

    public static Breadcrumb CleanseBreadcrumb(Breadcrumb b)
    {
        ArgumentNullException.ThrowIfNull(b);

        try
        {
            var message = CleanseLogMessage.Cleanse(b.Message ?? string.Empty);
            var data = b.Data?.ToDictionary(
                x => x.Key,
                y => CleanseLogMessage.Cleanse(y.Value ?? string.Empty));
            return new Breadcrumb(message, b.Type ?? "default", data, b.Category, b.Level);
        }
        catch (Exception)
        {
        }

        return b;
    }

    private static string? ShortenPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var rootDirs = new[] { "\\src\\", "/src/" };
        foreach (var rootDir in rootDirs)
        {
            var index = path.IndexOf(rootDir, StringComparison.Ordinal);

            if (index > 0)
            {
                return path.Substring(index + rootDir.Length - 1);
            }
        }

        return path;
    }
}
