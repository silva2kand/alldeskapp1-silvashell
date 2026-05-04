using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Web.WebView2.Wpf;
using SilvaShell.App.Core;

namespace SilvaShell.App.Plugins;

public interface IPlugin
{
    string Name { get; }
    string Description { get; }
    string Version { get; }
    string Author { get; }

    Task InitializeAsync();
    Task ShutdownAsync();
}

public interface IToolbarPlugin : IPlugin
{
    object GetToolbarControl();
}

public interface IMenuPlugin : IPlugin
{
    IEnumerable<MenuItem> GetMenuItems();
}

public interface IShortcutPlugin : IPlugin
{
    IEnumerable<PluginShortcut> GetShortcuts();
}

public interface IAppIntegrationPlugin : IPlugin
{
    void OnAppLoaded(AppModule app, WebView2 webView);
    void OnAppClosed(AppModule app);
}

public class PluginShortcut
{
    public string Name { get; set; } = "";
    public KeyGesture KeyGesture { get; set; } = new(Key.None);
    public Func<Task> Action { get; set; } = () => Task.CompletedTask;
}

public class PluginManifest
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Version { get; set; } = "";
    public string Author { get; set; } = "";
    public string AssemblyPath { get; set; } = "";
    public string ClassName { get; set; } = "";
    public List<string> Interfaces { get; set; } = new();
}