using Ookii.Dialogs.Wpf;
using SyncroCore;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfApp;

public partial class MainWindow : Window
{
    private string primaryDirectory;
    private string secondaryDirectory;
    private BitmapImage folderImageSource, copyImageSource, delImageSource;

    public MainWindow()
    {
        InitializeComponent();
        primaryDirectory = secondaryDirectory = "";
        folderImageSource = new BitmapImage(new Uri("Images/folder.ico", UriKind.Relative));
        copyImageSource = new BitmapImage(new Uri("Images/copy.ico", UriKind.Relative));
        delImageSource = new BitmapImage(new Uri("Images/del.ico", UriKind.Relative));
    }

    private void OpenPrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        VistaFolderBrowserDialog dialog = new();
        dialog.Description = "Please select a primary folder";
        dialog.UseDescriptionForTitle = true;

        if ((bool)dialog.ShowDialog(this))
        {
            primaryDirectory = dialog.SelectedPath;
            OpenPrimaryTextBox.Text = primaryDirectory;
        }
    }

    private void OpenSecondaryButton_Click(object sender, RoutedEventArgs e)
    {
        VistaFolderBrowserDialog dialog = new();
        dialog.Description = "Please select a secondary folder";
        dialog.UseDescriptionForTitle = true;

        if ((bool)dialog.ShowDialog(this))
        {
            secondaryDirectory = dialog.SelectedPath;
            OpenSecondaryTextBox.Text = secondaryDirectory;
        }
    }

    private void CompareButton_Click(object sender, RoutedEventArgs e)
    {
        primaryDirectory = "D:\\NeatData\\CurrentWorks\\syncro\\Code\\WpfApp\\bin\\Debug\\net6.0-windows\\source";
        secondaryDirectory = "D:\\NeatData\\CurrentWorks\\syncro\\Code\\WpfApp\\bin\\Debug\\net6.0-windows\\destination";

        if (Directory.Exists(primaryDirectory) && Directory.Exists(secondaryDirectory))
        {
            Synchronize synchronize = new();
            var synchInfo = synchronize.PrepareMirror(primaryDirectory, secondaryDirectory);

            DisplayedFileSystemEntryInfo displayedEmptyEntry = new() { };

            foreach (var file in synchInfo.SourceFilesToCopy)
            {
                DisplayedFileSystemEntryInfo displayedPrimaryEntry = new()
                {
                    Icon = ExtractFileIcon(file),
                    Name = file.Name,
                    Size = file.Size.ToString(),
                    LastWriteTime = file.LastWriteTime.ToString()
                };

                PrimaryDataGrid.Items.Add(displayedPrimaryEntry);
                OperationDataGrid.Items.Add(new DisplayedOperationEntryInfo() { Icon = copyImageSource });
                SecondaryDataGrid.Items.Add(displayedEmptyEntry);
            }

            foreach (var dir in synchInfo.SourceDirectoriesToCreate)
            {
                DisplayedFileSystemEntryInfo displayedPrimaryEntry = new()
                {
                    Icon = folderImageSource,
                    Name = dir.Name,
                    Size = "<Folder>",
                    LastWriteTime = ""
                };

                PrimaryDataGrid.Items.Add(displayedPrimaryEntry);
                OperationDataGrid.Items.Add(new DisplayedOperationEntryInfo() { Icon = copyImageSource });
                SecondaryDataGrid.Items.Add(displayedEmptyEntry);
            }

            foreach (var file in synchInfo.DestinationFilesToDelete)
            {
                DisplayedFileSystemEntryInfo displayedSecondaryEntry = new()
                {
                    Icon = ExtractFileIcon(file),
                    Name = file.Name,
                    Size = file.Size.ToString(),
                    LastWriteTime = file.LastWriteTime.ToString()
                };

                PrimaryDataGrid.Items.Add(displayedEmptyEntry);
                OperationDataGrid.Items.Add(new DisplayedOperationEntryInfo() { Icon = delImageSource });
                SecondaryDataGrid.Items.Add(displayedSecondaryEntry);
            }

            foreach (var dir in synchInfo.SourceDirectoriesToCreate)
            {
                DisplayedFileSystemEntryInfo displayedSecondaryEntry = new()
                {
                    Icon = folderImageSource,
                    Name = dir.Name,
                    Size = "<Folder>",
                    LastWriteTime = ""
                };

                PrimaryDataGrid.Items.Add(displayedEmptyEntry);
                OperationDataGrid.Items.Add(new DisplayedOperationEntryInfo() { Icon = delImageSource });
                SecondaryDataGrid.Items.Add(displayedSecondaryEntry);
            }
        }
    }

    private BitmapSource ExtractFileIcon(FileSystemEntryInfo file)
    {
        var filePath = Path.Combine(primaryDirectory, file.Name);
        var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
        var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                sysicon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        sysicon.Dispose();
        return bmpSrc;
    }
}

public record DisplayedFileSystemEntryInfo
{
    public BitmapSource Icon { get; set; }
    public string Name { get; set; }
    public string Size { get; set; }
    public string LastWriteTime { get; set; }
}

public record DisplayedOperationEntryInfo
{
    public BitmapSource Icon { get; set; }
}
