namespace SyncroCore;

public class FileSystemEntriesSet
{
    // Returns sets of files and directories
    public FileSystemInfo Get(string rootDir)
    {
        if (Directory.Exists(rootDir))
        {
            var dirInfo = new DirectoryInfo(rootDir);
            SortedSet<FileInfo> files;
            SortedSet<DirectoryInfo> dirs;
            FileSystemInfo fileSystemInfo = new();

            try
            {
                files = new SortedSet<FileInfo>(dirInfo.EnumerateFiles("*", SearchOption.AllDirectories));
                dirs = new SortedSet<DirectoryInfo>(dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories));
            }

            catch (Exception ex)
            {
                ex.Data.Add("Addendum", $"Exception occurred during Directory.EnumerateFiles({rootDir})");
                throw;
            }

            foreach (var file in files)
            {
                fileSystemInfo.FileSet.Add
                (new FileSystemEntryInfo
                {
                // Remove parent directory's name from relative path
                Name = file.Name[(file.Name.IndexOf(Path.DirectorySeparatorChar) + 1)..],
                    Length = file.Length,
                    LastWriteTime = file.LastWriteTime
                });
            }

            foreach (var dir in dirs)
            {
                fileSystemInfo.DirSet.Add
                (new FileSystemEntryInfo
                {
                // Remove parent directory's name from relative path
                Name = dir.Name[(dir.Name.IndexOf(Path.DirectorySeparatorChar) + 1)..]
                });
            }

            return fileSystemInfo;
        }

        throw new ArgumentException($"Given path \"{rootDir}\" not refers to an existing directory on disk.");
    }
}
