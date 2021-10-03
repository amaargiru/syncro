using System.Runtime.InteropServices;

namespace SyncroCore;

internal class FileSystemEntriesSet
{
    // Returns sets of files and directories
    internal SystemTreeInfo Get(string rootDir)
    {
        if (Directory.Exists(rootDir))
        {
            var dirInfo = new DirectoryInfo(rootDir);
            SortedSet<FileInfo> files;
            SortedSet<DirectoryInfo> dirs;
            SystemTreeInfo fileSystemInfo = new()
            {
                FileSet = new(),
                DirSet = new()
            };

            try
            {
                files = new SortedSet<FileInfo>(dirInfo.EnumerateFiles("*", SearchOption.AllDirectories), new SortByNameComparer());
            }

            catch (Exception ex)
            {
                ex.Data.Add("Addendum", $"Exception occurred during Directory.EnumerateFiles({rootDir})");
                throw;
            }

            try
            {
                dirs = new SortedSet<DirectoryInfo>(dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories), new SortByNameComparer());
            }

            catch (Exception ex)
            {
                ex.Data.Add("Addendum", $"Exception occurred during Directory.EnumerateDirectories({rootDir})");
                throw;
            }

            foreach (var file in files)
            {
                fileSystemInfo.FileSet.Add
                (new FileSystemEntryInfo
                {
                    // Remove parent directory's name from relative path
                    MiddleName = file.FullName.Replace(rootDir, ""),
                    ShortName = file.Name,
                    Size = file.Length,
                    LastWriteTime = file.LastWriteTime
                });
            }

            foreach (var dir in dirs)
            {
                fileSystemInfo.DirSet.Add
                (new FileSystemEntryInfo
                {
                    // Remove parent directory's name from relative path
                    MiddleName = dir.FullName.Replace(rootDir, ""),
                    ShortName = dir.Name
                });
            }

            return fileSystemInfo;
        }

        throw new ArgumentException($"Given path \"{rootDir}\" not refers to an existing directory on disk.");
    }

    // Defines a comparer to create a sorted set
    public class SortByNameComparer : IComparer<FileSystemInfo>
    {
        public int Compare(FileSystemInfo x, FileSystemInfo y)
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var comparer = isWindows ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            return comparer.Compare(x.Name, y.Name);
        }
    }
}
