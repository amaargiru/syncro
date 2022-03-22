using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SyncroCore;
using System.Threading.Channels;

namespace Syncro;

internal static class ConsoleSyncro
{
   const string logToConsoleOnly = "Processed:"; // Prefix for messages published in the console only
   const string logToFileOnly = "File log:"; // Prefix for messages saved to file only
   const string mainLogFile = @"log\syncro_main_log.txt";
   const string operationsLogFile = @"log\syncro_operations_log.txt";
   const string LoggingTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}";

   const int logLines = 4;
   private static async Task Main(string[] args)
    {
      var primaryDirectory = args[0];
      var secondaryDirectory = args[1];

      Log.Logger = new LoggerConfiguration()
                  .WriteTo.Logger(lc => lc
                  .Filter.ByExcluding(x => x.MessageTemplate.Text.StartsWith(logToConsoleOnly) || x.MessageTemplate.Text.StartsWith(logToFileOnly))
    .MinimumLevel.Debug()
    .WriteTo.Console(
         theme: AnsiConsoleTheme.Code,
         outputTemplate: LoggingTemplate)
    .WriteTo.File(mainLogFile,
        fileSizeLimitBytes: 1_000_000,
        retainedFileCountLimit: 5,
        rollOnFileSizeLimit: true,
        restrictedToMinimumLevel: LogEventLevel.Debug)
    )
         .WriteTo.Logger(lc => lc
         .Filter.ByIncludingOnly(x => x.MessageTemplate.Text.StartsWith(logToConsoleOnly))
    .MinimumLevel.Debug()
    .WriteTo.Console(
         theme: AnsiConsoleTheme.Code,
         outputTemplate: LoggingTemplate)
    )
                  .WriteTo.Logger(lc => lc
         .Filter.ByIncludingOnly(x => x.MessageTemplate.Text.StartsWith(logToFileOnly))
    .MinimumLevel.Debug()
    .WriteTo.File(operationsLogFile,
        fileSizeLimitBytes: 10_000_000,
        retainedFileCountLimit: 10,
        rollOnFileSizeLimit: true,
        restrictedToMinimumLevel: LogEventLevel.Debug)
    )
    .CreateLogger();

        Log.Information("Run Syncro...");

        var channel = Channel.CreateUnbounded<string>(); // Unbounded channel is for MVP only, use Bounded in real life
      Synchronize synchronize = new(channel);

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

            Log.Information($"Total items = {synchInfo.Count()}");

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

        while (await channel.Reader.WaitToReadAsync())
        {
            if (channel.Reader.TryRead(out var messageFromChannel))
            {
                Log.Information(logToFileOnly + " " + messageFromChannel); // Message will write to file
                logMessages.Enqueue(messageFromChannel); // Message will show in console temporarily

               var list = logMessages.ToList();

                ClearConsoleBelow(currentCursorY, depth);
                Console.SetCursorPosition(0, currentCursorY);

                foreach (var messageToConsole in list)
                {
                    Log.Information(logToConsoleOnly + " " + messageToConsole);
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
