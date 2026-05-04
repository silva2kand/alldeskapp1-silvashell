using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Web.WebView2.Wpf;
using SilvaShell.App.Core;
using SilvaShell.App.Plugins;

namespace SilvaShell.Plugins.NoteTaker;

public class NoteTakerPlugin : IPlugin, IToolbarPlugin, IShortcutPlugin, IAppIntegrationPlugin
{
    private TextBox? _notesTextBox;
    private string _currentNotesFile = "";
    private AppModule? _currentApp;

    public string Name => "Note Taker";
    public string Description => "Take notes while browsing different apps and websites";
    public string Version => "1.0.0";
    public string Author => "SilvaShell Team";

    public async Task InitializeAsync()
    {
        // Create notes directory
        Directory.CreateDirectory("Notes");

        // Plugin initialization logic
        await Task.CompletedTask;
    }

    public async Task ShutdownAsync()
    {
        // Save any unsaved notes
        await SaveCurrentNotesAsync();

        // Plugin cleanup logic
        await Task.CompletedTask;
    }

    public object GetToolbarControl()
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal };

        _notesTextBox = new TextBox
        {
            Width = 300,
            Height = 100,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Margin = new Thickness(5),
            Text = "Start taking notes..."
        };

        _notesTextBox.GotFocus += (s, e) =>
        {
            if (_notesTextBox.Text == "Start taking notes...")
            {
                _notesTextBox.Text = "";
            }
        };

        var saveButton = new Button
        {
            Content = "Save Notes",
            Margin = new Thickness(5),
            Padding = new Thickness(10, 5, 10, 5)
        };
        saveButton.Click += async (s, e) => await SaveCurrentNotesAsync();

        panel.Children.Add(new Label { Content = "📝 Notes:", Margin = new Thickness(5, 5, 0, 0) });
        panel.Children.Add(_notesTextBox);
        panel.Children.Add(saveButton);

        return panel;
    }

    public IEnumerable<PluginShortcut> GetShortcuts()
    {
        return new[]
        {
            new PluginShortcut
            {
                Name = "Save Notes",
                KeyGesture = new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift),
                Action = SaveCurrentNotesAsync
            },
            new PluginShortcut
            {
                Name = "Clear Notes",
                KeyGesture = new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift),
                Action = ClearNotesAsync
            }
        };
    }

    public void OnAppLoaded(AppModule app, WebView2 webView)
    {
        _currentApp = app;
        _currentNotesFile = $"Notes/{app.Id}_{DateTime.Now:yyyyMMdd}.txt";

        // Load existing notes for this app/date
        if (File.Exists(_currentNotesFile))
        {
            try
            {
                var notes = File.ReadAllText(_currentNotesFile);
                if (_notesTextBox != null)
                {
                    _notesTextBox.Text = notes;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading notes: {ex.Message}");
            }
        }
        else if (_notesTextBox != null)
        {
            _notesTextBox.Text = $"Notes for {app.Name} ({DateTime.Now:d})...\n\n";
        }
    }

    public void OnAppClosed(AppModule app)
    {
        // Auto-save when switching apps
        Task.Run(() => SaveCurrentNotesAsync());
    }

    private async Task SaveCurrentNotesAsync()
    {
        if (_notesTextBox == null || string.IsNullOrWhiteSpace(_notesTextBox.Text))
            return;

        try
        {
            await File.WriteAllTextAsync(_currentNotesFile, _notesTextBox.Text);
            System.Diagnostics.Debug.WriteLine($"Notes saved to {_currentNotesFile}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving notes: {ex.Message}");
        }
    }

    private async Task ClearNotesAsync()
    {
        if (_notesTextBox != null)
        {
            _notesTextBox.Text = "";
        }
        await Task.CompletedTask;
    }
}