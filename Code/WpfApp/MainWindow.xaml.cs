using Ookii.Dialogs.Wpf;
using SyncroCore;
using System.IO;
using System.Windows;

namespace WpfApp;

public partial class MainWindow : Window
{
    private string primaryDirectory;
    private string secondaryDirectory;

    public MainWindow()
    {
        InitializeComponent();
        primaryDirectory = secondaryDirectory = "";
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
        if (Directory.Exists(primaryDirectory) && Directory.Exists(secondaryDirectory))
        {
            Synchronize synchronize = new();
            var synchInfo = synchronize.PrepareMirror(primaryDirectory, secondaryDirectory);
        }
    }
}
