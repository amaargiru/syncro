using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SyncroCore;
using System.Threading.Channels;

namespace Syncro;

internal static class ConsoleSyncro
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}")
            .WriteTo.File(@"log\syncro_log.txt",
                fileSizeLimitBytes: 10_000_000,
                retainedFileCountLimit: 5,
                rollOnFileSizeLimit: true,
                restrictedToMinimumLevel: LogEventLevel.Debug)
            .CreateLogger();

        Log.Information("Run Syncro...");

        const int logLines = 4;

        var channel = Channel.CreateUnbounded<string>();
        Synchronize synchronize = new(channel);

        var primaryDirectory = @"C:\Example of two diff dirs 2\DirA";
        var secondaryDirectory = @"C:\Example of two diff dirs 2\DirB";

        if (!Directory.Exists(primaryDirectory))
        {
            Console.WriteLine($"Primary directory {primaryDirectory} doesn't exist");
        }
        else if (!Directory.Exists(secondaryDirectory))
        {
            Console.WriteLine($"Secondary directory {secondaryDirectory} doesn't exist");
        }
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

            var synchInfo = synchronize.PrepareMirror(primaryDirectory, secondaryDirectory);

            var totalItemsToDo = synchInfo.Count();
            Log.Information($"Total items = {totalItemsToDo}");

            var currentCursorPosition = Console.GetCursorPosition();

            MessageLoggerAsync(channel, logLines, currentCursorPosition.Top);

            await synchronize.DeleteSecondaryFilesAsync(secondaryDirectory, synchInfo);
            await synchronize.DeleteSecondaryDirectories(secondaryDirectory, synchInfo);
            await synchronize.CreateSecondaryDirectories(secondaryDirectory, synchInfo);
            await synchronize.CopyFilesFromPrimaryToSecondary(primaryDirectory, secondaryDirectory, synchInfo);

            Log.Information("All done.");

            Console.Read();
        }
    }

    private static async Task MessageLoggerAsync(Channel<string> channel, int logLines, int currentCursorY)
    {
        FixedSizeQueue<string> logMessages = new()
        {
            Limit = logLines
        };

        var depth = currentCursorY;
        var item = 1;

        while (await channel.Reader.WaitToReadAsync())
        {
            if (channel.Reader.TryRead(out var messageFromChannel))
            {
                //Log.Information(messageFromChannel);
                logMessages.Enqueue(item++ + " " + messageFromChannel);

                var list = logMessages.ToList();

                ClearConsoleBelow(currentCursorY, depth);
                Console.SetCursorPosition(0, currentCursorY);

                foreach (var messageToConsole in list)
                {
                    Log.Verbose(messageToConsole);
                    depth = Console.GetCursorPosition().Top;
                }
            }
        }
    }

    private static void ClearConsoleBelow(int y, int depth)
    {
        for (var i = y; i < depth; i++)
        {
            ClearConsoleString(i);
        }
    }

    private static void ClearConsoleString(int y)
    {
        Console.SetCursorPosition(0, y);
        Console.Write(new string(' ', Console.BufferWidth));
        Console.SetCursorPosition(0, y);
    }
}
