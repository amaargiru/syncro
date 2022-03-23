namespace SyncroCore;
using System.Threading.Channels;

public class Synchronize
{
    private Channel<string> _channel;

    public Synchronize(Channel<string> channel)
    {
        this._channel = channel;
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
            var fullSecondaryFilePath = secondaryDirectory + file.MiddleName;

         _ = new FileInfo(fullSecondaryFilePath)
         {
            IsReadOnly = false
         };

         // This file may have been deleted during processing
         if (File.Exists(fullSecondaryFilePath))
         {
            File.Delete(fullSecondaryFilePath);
            await _channel.Writer.WriteAsync($"[DEL] File {fullSecondaryFilePath} has been deleted");
         }
        }
    }

    public async Task DeleteSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var dir in synchInfo.SecondaryDirectoriesToDelete)
        {
            var fullSecondaryDirPath = secondaryDirectory + dir.MiddleName;

            // This folder may have been deleted during processing
            if (Directory.Exists(fullSecondaryDirPath))
            {
                Directory.Delete(fullSecondaryDirPath, recursive: true);
                await _channel.Writer.WriteAsync($"[DEL] Directory {fullSecondaryDirPath} has been deleted");
            }
        }
    }

    public async Task CreateSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var dir in synchInfo.SecondaryDirectoriesToCreate)
        {
            var fullSecondaryDirPath = secondaryDirectory + dir.MiddleName;

            Directory.CreateDirectory(fullSecondaryDirPath);
            await _channel.Writer.WriteAsync($"[CREATE] Directory {fullSecondaryDirPath} has been created");
        }
    }

    public async Task CopyFilesFromPrimaryToSecondary(string primaryDirectory, string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var file in synchInfo.PrimaryFilesToCopy)
        {
            var fullPrimaryFilePath = primaryDirectory + file.MiddleName;
            var fullSecondaryFilePath = secondaryDirectory + file.MiddleName;

            File.Copy(fullPrimaryFilePath, fullSecondaryFilePath, overwrite: true);
            await _channel.Writer.WriteAsync($"[COPY] File {fullPrimaryFilePath} has been copied to {fullSecondaryFilePath}");
        }

      await _channel.Writer.WriteAsync("[DONE]");
   }
}
