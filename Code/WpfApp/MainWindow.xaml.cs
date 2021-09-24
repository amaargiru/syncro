using Ookii.Dialogs.Wpf;
using System.Windows;

namespace WpfApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenPrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        VistaFolderBrowserDialog dialog = new();
        dialog.Description = "Please select a folder.";
        dialog.UseDescriptionForTitle = true;

        if ((bool)dialog.ShowDialog(this))
        {
            MessageBox.Show(this, "The selected folder is: " + dialog.SelectedPath, "Sample folder browser dialog");
        }
    }
}
