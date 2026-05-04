using System.Windows;
using SilvaShell.App.Workspaces;

namespace SilvaShell.App.Views;

public partial class WorkspaceManagerWindow : Window
{
    private readonly WorkspaceManager _workspaceManager;
    private readonly MainWindow _mainWindow;

    public WorkspaceManagerWindow(WorkspaceManager workspaceManager, MainWindow mainWindow)
    {
        InitializeComponent();
        _workspaceManager = workspaceManager;
        _mainWindow = mainWindow;
        LoadWorkspaces();
    }

    private void LoadWorkspaces()
    {
        WorkspacesListView.ItemsSource = _workspaceManager.Workspaces;
    }

    private async void LoadWorkspaceButton_Click(object sender, RoutedEventArgs e)
    {
        if (WorkspacesListView.SelectedItem is Workspace selectedWorkspace)
        {
            try
            {
                await _mainWindow.LoadWorkspaceAsync(selectedWorkspace);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading workspace: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Please select a workspace to load.", "No Selection",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void DeleteWorkspaceButton_Click(object sender, RoutedEventArgs e)
    {
        if (WorkspacesListView.SelectedItem is Workspace selectedWorkspace)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete workspace '{selectedWorkspace.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _workspaceManager.DeleteWorkspace(selectedWorkspace.Name);
                LoadWorkspaces();
            }
        }
        else
        {
            MessageBox.Show("Please select a workspace to delete.", "No Selection",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}