using System.Runtime.InteropServices;

namespace SyncroCore;

internal class FileSystemEntriesSet
{
   /// Returns sets of files and directories
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

         var files = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);

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

         var dirs = dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories);

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
}
