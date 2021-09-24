namespace SyncroCore;

public class Synchronize
{
    public SynchInfo PrepareMirror(string sourceDirectory, string destinationDirectory)
    {
        FileSystemEntriesSet fileSystemEntriesSet = new();
        SynchInfo synchInfo = new();

        var sourceSystemInfo = fileSystemEntriesSet.Get(sourceDirectory);
        var destinationSystemInfo = fileSystemEntriesSet.Get(destinationDirectory);

        synchInfo.DestinationFilesToDelete = destinationSystemInfo.FileSet.Clone();
        synchInfo.DestinationFilesToDelete.ExceptWith(sourceSystemInfo.FileSet);

        synchInfo.DestinationDirectoriesToDelete = destinationSystemInfo.DirSet.Clone();
        synchInfo.DestinationDirectoriesToDelete.ExceptWith(sourceSystemInfo.DirSet);

        synchInfo.SourceDirectoriesToCreate = sourceSystemInfo.DirSet.Clone();
        synchInfo.SourceDirectoriesToCreate.ExceptWith(destinationSystemInfo.DirSet);

        synchInfo.SourceFilesToCopy = sourceSystemInfo.FileSet.Clone();
        synchInfo.SourceFilesToCopy.ExceptWith(destinationSystemInfo.FileSet);

        return synchInfo;
    }
}
