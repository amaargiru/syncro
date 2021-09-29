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
    private BitmapImage myImageSource;

    public MainWindow()
    {
        InitializeComponent();
        primaryDirectory = secondaryDirectory = "";
        myImageSource = new BitmapImage(new Uri("Images/folder.ico", UriKind.Relative));
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
                // Extract file icon
                var filePath = Path.Combine(primaryDirectory, file.Name);
                var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
                var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        sysicon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                sysicon.Dispose();

                DisplayedFileSystemEntryInfo displayedPrimaryEntry = new()
                {
                    Icon = bmpSrc,
                    Name = file.Name,
                    Size = file.Size.ToString(),
                    LastWriteTime = file.LastWriteTime.ToString()
                };

                PrimaryDataGrid.Items.Add(displayedPrimaryEntry);
                SecondaryDataGrid.Items.Add(displayedEmptyEntry);
            }

            foreach (var dir in synchInfo.SourceDirectoriesToCreate)
            {
                DisplayedFileSystemEntryInfo displayedEntry = new()
                {
                    Icon = myImageSource,
                    Name = dir.Name,
                    Size = "",
                    LastWriteTime = ""
                };

                PrimaryDataGrid.Items.Add(displayedEntry);
                SecondaryDataGrid.Items.Add(displayedEmptyEntry);
            }
        }
    }
}

public record DisplayedFileSystemEntryInfo
{
    public BitmapSource Icon { get; set; }
    public string Name { get; set; }
    public string Size { get; set; }
    public string LastWriteTime { get; set; }
}
