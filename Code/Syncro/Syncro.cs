using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SyncroCore;
using System.Threading.Channels;

namespace Syncro;

internal static class ConsoleSyncro
{
    private static async Task Main()
    {

        Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(
         theme: AnsiConsoleTheme.Code,
         outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}")
    .WriteTo.File(@"log\log2048.txt",
        fileSizeLimitBytes: 1000_000,
        retainedFileCountLimit: 5,
        rollOnFileSizeLimit: true)
    .CreateLogger();

        Log.Debug("Run Syncro...");

        var logLines = 4;

        Channel<string> channel = Channel.CreateUnbounded<string>();
        Synchronize synchronize = new(channel);
        SynchInfo synchInfo;

        var primaryDirectory = @"D:\Polygon\Example of two diff dirs 2\DirA";
        var secondaryDirectory = @"D:\Polygon\Example of two diff dirs 2\DirB";

        if (!Directory.Exists(primaryDirectory))
            Console.WriteLine($"Primary directory {primaryDirectory} doesn't exist");
        else if (!Directory.Exists(secondaryDirectory))
            Console.WriteLine($"Secondary directory {secondaryDirectory} doesn't exist");
        else
        {
            if (!Path.IsPathFullyQualified(primaryDirectory))
            {
                primaryDirectory = Path.GetFullPath(primaryDirectory);
            }

            if (!Path.IsPathFullyQualified(secondaryDirectory))
            {
                secondaryDirectory = Path.GetFullPath(secondaryDirectory);
            }

            synchInfo = synchronize.PrepareMirror(primaryDirectory, secondaryDirectory);

            var totalItemsToDo = synchInfo.Count();
            Log.Debug($"Total items = {totalItemsToDo}");

            var currentCursorPosition = Console.GetCursorPosition();

            MessageLoggerAsync(channel, logLines, currentCursorPosition.Top);

            await synchronize.DeleteSecondaryFilesAsync(secondaryDirectory, synchInfo);
            await synchronize.DeleteSecondaryDirectories(secondaryDirectory, synchInfo);
            await synchronize.CreateSecondaryDirectories(secondaryDirectory, synchInfo);
            await synchronize.CopyFilesFromPrimaryToSecondary(primaryDirectory, secondaryDirectory, synchInfo);

            Log.Debug("All done.");

            Console.Read();
        }
    }

    static async Task MessageLoggerAsync(Channel<string> channel, int logLines, int currentCursorY)
    {
        FixedSizeQueue<string> log_messages = new();
        log_messages.Limit = logLines;

        var depth = currentCursorY;
        var item = 1;

        while (await channel.Reader.WaitToReadAsync())
        {
            if (channel.Reader.TryRead(out var msg))
            {
                log_messages.Enqueue(item++ + " " + msg);

                var list = log_messages.ToList();

                ClearConsoleBelow(currentCursorY, depth);
                Console.SetCursorPosition(0, currentCursorY);

                foreach (var m in list)
                {
                    if (m != null)
                    {
                        Log.Debug(m);
                        depth = Console.GetCursorPosition().Top;
                    }
                }
            }
        }
    }

    static void ClearConsoleBelow(int Y, int depth)
    {
        for (var i = Y; i < depth; i++)
        {
            ClearConsoleString(i);
        }
    }

    static void ClearConsoleString(int Y)
    {
        Console.SetCursorPosition(0, Y);
        Console.Write(new string(' ', Console.BufferWidth));
        Console.SetCursorPosition(0, Y);
    }
}
