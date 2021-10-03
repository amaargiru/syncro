namespace SyncroCore;

public class Synchronize
{
    public SynchInfo PrepareMirror(string primaryDirectory, string secondaryDirectory)
    {
        FileSystemEntriesSet fileSystemEntriesSet = new();
        SynchInfo synchInfo = new();

        var sourceSystemInfo = fileSystemEntriesSet.Get(primaryDirectory);
        var destinationSystemInfo = fileSystemEntriesSet.Get(secondaryDirectory);

        synchInfo.DestinationFilesToDelete = destinationSystemInfo.FileSet.Clone();
        synchInfo.DestinationFilesToDelete.ExceptWith(sourceSystemInfo.FileSet);

        synchInfo.DestinationDirectoriesToDelete = destinationSystemInfo.DirSet.Clone();
        synchInfo.DestinationDirectoriesToDelete.ExceptWith(sourceSystemInfo.DirSet);

        synchInfo.DestinationDirectoriesToCreate = sourceSystemInfo.DirSet.Clone();
        synchInfo.DestinationDirectoriesToCreate.ExceptWith(destinationSystemInfo.DirSet);

        synchInfo.SourceFilesToCopy = sourceSystemInfo.FileSet.Clone();
        synchInfo.SourceFilesToCopy.ExceptWith(destinationSystemInfo.FileSet);

        return synchInfo;
    }

    public void DeleteSecondaryFiles(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var file in synchInfo.DestinationFilesToDelete)
        {
            File.Delete(secondaryDirectory + file.MiddleName);
        }
    }

    public void DeleteSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var dir in synchInfo.DestinationDirectoriesToDelete)
        {
            // This folder may have been deleted by a previous operation as a subfolder
            if (Directory.Exists(secondaryDirectory + dir.MiddleName))
            {
                Directory.Delete(secondaryDirectory + dir.MiddleName, recursive: true);
            }
        }
    }

    public void CreateSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var dir in synchInfo.DestinationDirectoriesToCreate)
        {
            Directory.CreateDirectory(secondaryDirectory + dir.MiddleName);
        }
    }

    public void CopyFilesFromPrimaryToSecondary(string primaryDirectory, string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var file in synchInfo.SourceFilesToCopy)
        {
            File.Copy(primaryDirectory + file.MiddleName, secondaryDirectory + file.MiddleName, true);
        }
    }
}
