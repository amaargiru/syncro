namespace SyncroCore;
using Serilog;

public class Synchronize
{
   public SynchInfo PrepareMirror(string primaryDirectory, string secondaryDirectory)
   {
      FileSystemEntriesSet fileSystemEntriesSet = new();
      SynchInfo synchInfo = new();

      var primarySystemInfo = fileSystemEntriesSet.Get(primaryDirectory);
      var secondarySystemInfo = fileSystemEntriesSet.Get(secondaryDirectory);

      synchInfo.SecondaryFilesToDelete = secondarySystemInfo.FileSet.Clone();
      synchInfo.SecondaryFilesToDelete.ExceptWith(primarySystemInfo.FileSet);

      synchInfo.SecondaryDirectoriesToDelete = secondarySystemInfo.DirSet.Clone();
      synchInfo.SecondaryDirectoriesToDelete.ExceptWith(primarySystemInfo.DirSet);

      synchInfo.SecondaryDirectoriesToCreate = primarySystemInfo.DirSet.Clone();
      synchInfo.SecondaryDirectoriesToCreate.ExceptWith(secondarySystemInfo.DirSet);

      synchInfo.PrimaryFilesToCopy = primarySystemInfo.FileSet.Clone();
      synchInfo.PrimaryFilesToCopy.ExceptWith(secondarySystemInfo.FileSet);

      return synchInfo;
   }

   public void DeleteSecondaryFiles(string secondaryDirectory, SynchInfo synchInfo)
   {
      foreach (var file in synchInfo.SecondaryFilesToDelete)
      {
         var fullFilePath = secondaryDirectory + file.MiddleName;

         FileInfo fInfo = new FileInfo(fullFilePath);
         fInfo.IsReadOnly = false;
         File.Delete(fullFilePath);
         Log.Debug($"[DEL] File {fullFilePath} has been deleted");
      }
   }

   public void DeleteSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
   {
      foreach (var dir in synchInfo.SecondaryDirectoriesToDelete)
      {
         var fullDirPath = secondaryDirectory + dir.MiddleName;

         // This folder may have been deleted by a previous operation as a subfolder
         if (Directory.Exists(fullDirPath))
         {
            Directory.Delete(fullDirPath, recursive: true);
            Log.Debug($"[DEL] Directory {fullDirPath} has been deleted");
         }
      }
   }

   public void CreateSecondaryDirectories(string secondaryDirectory, SynchInfo synchInfo)
   {
      foreach (var dir in synchInfo.SecondaryDirectoriesToCreate)
      {
         var fullDirPath = secondaryDirectory + dir.MiddleName;

         Directory.CreateDirectory(fullDirPath);
         Log.Debug($"[CREATE] Directory {fullDirPath} has been created");
      }
   }

   public void CopyFilesFromPrimaryToSecondary(string primaryDirectory, string secondaryDirectory, SynchInfo synchInfo)
   {
      foreach (var file in synchInfo.PrimaryFilesToCopy)
      {
         var fullFilePath = primaryDirectory + file.MiddleName;

         File.Copy(primaryDirectory + file.MiddleName, secondaryDirectory + file.MiddleName, true);
         Log.Debug($"[COPY] File {fullFilePath} has been copied");
      }
   }
}
