namespace SyncroCore;

public record SystemTreeInfo
{
    public SortedSet<FileSystemEntryInfo> FileSet { get; set; }
    public SortedSet<FileSystemEntryInfo> DirSet { get; set; }
}

public record SynchInfo
{
    public SortedSet<FileSystemEntryInfo> DestinationFilesToDelete { get; set; }
    public SortedSet<FileSystemEntryInfo> DestinationDirectoriesToDelete { get; set; }
    public SortedSet<FileSystemEntryInfo> DestinationDirectoriesToCreate { get; set; }
    public SortedSet<FileSystemEntryInfo> SourceFilesToCopy { get; set; }
}
