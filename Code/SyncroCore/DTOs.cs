namespace SyncroCore;

public record FileSystemInfo
{
    public SortedSet<FileSystemEntryInfo> FileSet { get; set; }
    public SortedSet<FileSystemEntryInfo> DirSet { get; set; }
}

public record SynchInfo
{
    public SortedSet<FileSystemEntryInfo> DestinationFilesToDelete { get; set; }
    public SortedSet<FileSystemEntryInfo> DestinationDirectoriesToDelete { get; set; }
    public SortedSet<FileSystemEntryInfo> SourceDirectoriesToCreate { get; set; }
    public SortedSet<FileSystemEntryInfo> SourceFilesToCopy { get; set; }
}
