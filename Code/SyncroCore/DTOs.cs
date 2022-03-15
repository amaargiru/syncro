namespace SyncroCore;

public record SystemTreeInfo
{
    public SortedSet<FileSystemEntryInfo>? FileSet { get; set; }
    public SortedSet<FileSystemEntryInfo>? DirSet { get; set; }
}

public record SynchInfo
{
    public SortedSet<FileSystemEntryInfo>? SecondaryFilesToDelete { get; set; }
    public SortedSet<FileSystemEntryInfo>? SecondaryDirectoriesToDelete { get; set; }
    public SortedSet<FileSystemEntryInfo>? SecondaryDirectoriesToCreate { get; set; }
    public SortedSet<FileSystemEntryInfo>? PrimaryFilesToCopy { get; set; }
}
