namespace SyncroCore;
using Serilog;
using System.Threading.Channels;

public class Synchronize
{
    Channel<string> channel;

    public Synchronize(Channel<string> channel)
    {
        this.channel = channel;
    }

    public SynchInfo PrepareMirror(string primaryDirectory, string secondaryDirectory)
    {
        FileSystemEntriesSet fileSystemEntriesSet = new();
        SynchInfo synchInfo = new();

        var primarySystemInfo = fileSystemEntriesSet.Get(primaryDirectory);
        var secondarySystemInfo = fileSystemEntriesSet.Get(secondaryDirectory);

        synchInfo.SecondaryFilesToDelete = secondarySystemInfo.FileSet.Clone();
        synchInfo.SecondaryFilesToDelete.ExceptWith(primarySystemInfo.FileSet);

        synchInfo.SecondaryDirectoriesToDelete = secondarySystemInfo.DirSet.Clone();
        synchInfo.SecondaryDirectoriesToDelete.ExceptWith(primarySystemInfo.DirSet);

        synchInfo.SecondaryDirectoriesToCreate = primarySystemInfo.DirSet.Clone();
        synchInfo.SecondaryDirectoriesToCreate.ExceptWith(secondarySystemInfo.DirSet);

        synchInfo.PrimaryFilesToCopy = primarySystemInfo.FileSet.Clone();
        synchInfo.PrimaryFilesToCopy.ExceptWith(secondarySystemInfo.FileSet);

        return synchInfo;
    }

    public async Task DeleteSecondaryFilesAsync(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var file in synchInfo.SecondaryFilesToDelete)
        {
            var fullFilePath = secondaryDirectory + file.MiddleName;

            FileInfo fInfo = new FileInfo(fullFilePath);
            fInfo.IsReadOnly = false;
            File.Delete(fullFilePath);
            await channel.Writer.WriteAsync($"[DEL] File {fullFilePath} has been deleted");
        }
    }

    public async Task DeleteSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var dir in synchInfo.SecondaryDirectoriesToDelete)
        {
            var fullDirPath = secondaryDirectory + dir.MiddleName;

            // This folder may have been deleted by a previous operation as a subfolder
            if (Directory.Exists(fullDirPath))
            {
                Directory.Delete(fullDirPath, recursive: true);
                await channel.Writer.WriteAsync($"[DEL] Directory {fullDirPath} has been deleted");
            }
        }
    }

    public async Task CreateSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var dir in synchInfo.SecondaryDirectoriesToCreate)
        {
            var fullDirPath = secondaryDirectory + dir.MiddleName;

            Directory.CreateDirectory(fullDirPath);
            await channel.Writer.WriteAsync($"[CREATE] Directory {fullDirPath} has been created");
        }
    }

    public async Task CopyFilesFromPrimaryToSecondary(string primaryDirectory, string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var file in synchInfo.PrimaryFilesToCopy)
        {
            var fullFilePath = primaryDirectory + file.MiddleName;

            File.Copy(primaryDirectory + file.MiddleName, secondaryDirectory + file.MiddleName, true);
            await channel.Writer.WriteAsync($"[COPY] File {fullFilePath} has been copied");
        }
    }
}
