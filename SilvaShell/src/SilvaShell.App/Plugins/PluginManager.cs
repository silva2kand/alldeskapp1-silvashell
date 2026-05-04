using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilvaShell.App.Plugins;

public class PluginManager
{
    private readonly string _pluginsDirectory;
    private readonly ObservableCollection<IPlugin> _loadedPlugins = new();

    public IReadOnlyCollection<IPlugin> LoadedPlugins => _loadedPlugins;

    public PluginManager(string pluginsDirectory = "Plugins")
    {
        _pluginsDirectory = pluginsDirectory;
        Directory.CreateDirectory(_pluginsDirectory);
    }

    public async Task LoadPluginsAsync()
    {
        var pluginDirectories = Directory.GetDirectories(_pluginsDirectory);

        foreach (var pluginDir in pluginDirectories)
        {
            try
            {
                await LoadPluginFromDirectoryAsync(pluginDir);
            }
            catch (Exception ex)
            {
                // Log error but continue loading other plugins
                System.Diagnostics.Debug.WriteLine($"Failed to load plugin from {pluginDir}: {ex.Message}");
            }
        }
    }

    private async Task LoadPluginFromDirectoryAsync(string pluginDir)
    {
        var manifestPath = Path.Combine(pluginDir, "plugin.json");

        if (!File.Exists(manifestPath))
            return;

        var manifestJson = await File.ReadAllTextAsync(manifestPath);
        var manifest = JsonSerializer.Deserialize<PluginManifest>(manifestJson);

        if (manifest == null || string.IsNullOrEmpty(manifest.AssemblyPath) || string.IsNullOrEmpty(manifest.ClassName))
            return;

        var assemblyPath = Path.Combine(pluginDir, manifest.AssemblyPath);

        if (!File.Exists(assemblyPath))
            return;

        var assembly = Assembly.LoadFrom(assemblyPath);
        var pluginType = assembly.GetType(manifest.ClassName);

        if (pluginType == null)
            return;

        var plugin = Activator.CreateInstance(pluginType) as IPlugin;

        if (plugin != null)
        {
            await plugin.InitializeAsync();
            _loadedPlugins.Add(plugin);
        }
    }

    public async Task UnloadPluginsAsync()
    {
        foreach (var plugin in _loadedPlugins)
        {
            try
            {
                await plugin.ShutdownAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error shutting down plugin {plugin.Name}: {ex.Message}");
            }
        }

        _loadedPlugins.Clear();
    }

    public IEnumerable<T> GetPluginsOfType<T>() where T : IPlugin
    {
        return _loadedPlugins.OfType<T>();
    }
}