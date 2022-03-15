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
         SystemTreeInfo fileSystemInfo = new()
         {
            FileSet = new(),
            DirSet = new()
         };

         SortedSet<FileInfo> files = new SortedSet<FileInfo>(dirInfo.EnumerateFiles("*", SearchOption.AllDirectories), new SortByNameComparer());
         SortedSet<DirectoryInfo> dirs = new SortedSet<DirectoryInfo>(dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories), new SortByNameComparer());

         foreach (var file in files)
         {
            fileSystemInfo.FileSet.Add
            (new FileSystemEntryInfo
            {
               MiddleName = file.FullName.Replace(rootDir, ""), // Remove parent directory's name from relative path
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
               MiddleName = dir.FullName.Replace(rootDir, ""), // Remove parent directory's name from relative path
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
      public int Compare(FileSystemInfo? info1, FileSystemInfo? info2)
      {
         var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
         var comparer = isWindows ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

         return comparer.Compare(info1?.Name, info2?.Name);
      }
   }
}
