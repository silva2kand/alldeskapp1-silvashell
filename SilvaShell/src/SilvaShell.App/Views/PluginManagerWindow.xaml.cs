using System.Windows;
using SilvaShell.App.Plugins;

namespace SilvaShell.App.Views;

public partial class PluginManagerWindow : Window
{
    private readonly PluginManager _pluginManager;

    public PluginManagerWindow(PluginManager pluginManager)
    {
        InitializeComponent();
        _pluginManager = pluginManager;
        LoadPlugins();
    }

    private void LoadPlugins()
    {
        PluginsListView.ItemsSource = _pluginManager.LoadedPlugins;
    }

    private async void ReloadPluginsButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _pluginManager.UnloadPluginsAsync();
            await _pluginManager.LoadPluginsAsync();
            LoadPlugins();

            MessageBox.Show("Plugins reloaded successfully!", "Plugin Manager",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reloading plugins: {ex.Message}", "Plugin Manager",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}