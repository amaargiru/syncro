using SyncroCore;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Threading.Channels;

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

    var a = Console.GetCursorPosition();
    Console.SetCursorPosition(0, a.Top);

    await synchronize.DeleteSecondaryFilesAsync(secondaryDirectory, synchInfo);
    await synchronize.DeleteSecondaryDirectories(secondaryDirectory, synchInfo);
    await synchronize.CreateSecondaryDirectories(secondaryDirectory, synchInfo);
    await synchronize.CopyFilesFromPrimaryToSecondary(primaryDirectory, secondaryDirectory, synchInfo);

    while (await channel.Reader.WaitToReadAsync())
    {
        if (channel.Reader.TryRead(out var msg))
        {
            Console.WriteLine("Info from channel" + msg);
        }
    }
}

Console.Read();