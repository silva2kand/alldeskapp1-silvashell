using System.IO;
using System.Windows;
using System.Text.Json;
using SilvaShell.App.Core;

namespace SilvaShell.App.Views;

public partial class AddAppWindow : Window
{
    public AddAppWindow()
    {
        InitializeComponent();
    }

    private void BrowseIcon_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.ico",
            Title = "Select Icon"
        };

        if (dialog.ShowDialog() == true)
        {
            IconBox.Text = dialog.FileName;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var name = NameBox.Text?.Trim();
            var url = UrlBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter app name and URL.");
                return;
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            var newApp = new AppModule
            {
                Id = name.ToLower().Replace(" ", "-"),
                Name = name,
                Url = url,
                Icon = string.IsNullOrWhiteSpace(IconBox.Text) ? "" : IconBox.Text,
                AllowTerminal = TerminalCheck.IsChecked ?? false,
                RequiresLogin = false,
                RegionSensitive = false,
                LanguageFocus = "global",
                Notes = "Custom added app",
                Group = "Other"
            };

            // Get correct config path
            var configDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            
            var configPath = Path.Combine(configDir, "apps.json");

            // Load existing apps
            var apps = AppRegistry.Load(configPath).ToList();
            
            // Check for duplicates
            if (apps.Any(a => a.Id == newApp.Id))
            {
                newApp.Id += "-" + DateTime.Now.Ticks.ToString().Substring(10);
            }
            
            apps.Add(newApp);

            // Save back to JSON
            var json = JsonSerializer.Serialize(apps, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding app: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
