namespace SyncroCore;

// Light weight alternative for FileInfo and DirectoryInfo
public record FileSystemEntryInfo : IComparable<FileSystemEntryInfo>
{
    public string? ShortName { get; set; }
    public string? MiddleName { get; set; }
    public long Size { get; set; }
    public DateTime LastWriteTime { get; set; }

    public int CompareTo(FileSystemEntryInfo? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        var nameComparison = string.CompareOrdinal(MiddleName, other.MiddleName);
        if (nameComparison != 0)
        {
            return nameComparison;
        }

        var lengthComparison = Size.CompareTo(other.Size);
        if (lengthComparison != 0)
        {
            return lengthComparison;
        }

        return LastWriteTime.CompareTo(other.LastWriteTime);
    }
}
