namespace SyncroCore;

// Light weight alternative for FileInfo and DirectoryInfo
public record FileSystemEntryInfo : IComparable<FileSystemEntryInfo>
{
    public string Name { get; set; }
    public long Length { get; set; }
    public DateTime LastWriteTime { get; set; }

    public int CompareTo(FileSystemEntryInfo other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        var nameComparison = string.CompareOrdinal(Name, other.Name);
        if (nameComparison != 0)
        {
            return nameComparison;
        }

        var lengthComparison = Length.CompareTo(other.Length);
        if (lengthComparison != 0)
        {
            return lengthComparison;
        }

        return LastWriteTime.CompareTo(other.LastWriteTime);
    }
}
