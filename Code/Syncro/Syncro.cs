using SyncroCore;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

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

Synchronize synchronize = new();
SynchInfo synchInfo;

var primaryDirectory = @"Example of two diff dirs\DirA";
var secondaryDirectory = @"Example of two diff dirs\DirB";

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

   synchronize.DeleteSecondaryFiles(secondaryDirectory, synchInfo);
   synchronize.DeleteSecondaryDirectories(secondaryDirectory, synchInfo);
   synchronize.CreateSecondaryDirectories(secondaryDirectory, synchInfo);
   synchronize.CopyFilesFromPrimaryToSecondary(primaryDirectory, secondaryDirectory, synchInfo);
}

Console.Read();