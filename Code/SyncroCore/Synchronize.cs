namespace SyncroCore;

public class Synchronize
{
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

    public void DeleteSecondaryFiles(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var file in synchInfo.SecondaryFilesToDelete)
        {
            File.Delete(secondaryDirectory + file.MiddleName);
        }
    }

    public void DeleteSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var dir in synchInfo.SecondaryDirectoriesToDelete)
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
        foreach (var dir in synchInfo.SecondaryDirectoriesToCreate)
        {
            Directory.CreateDirectory(secondaryDirectory + dir.MiddleName);
        }
    }

    public void CopyFilesFromPrimaryToSecondary(string primaryDirectory, string secondaryDirectory, SynchInfo synchInfo)
    {
        foreach (var file in synchInfo.PrimaryFilesToCopy)
        {
            File.Copy(primaryDirectory + file.MiddleName, secondaryDirectory + file.MiddleName, true);
        }
    }
}
