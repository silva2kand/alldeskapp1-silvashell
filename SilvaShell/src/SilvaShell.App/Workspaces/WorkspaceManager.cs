using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilvaShell.App.Workspaces;

public class WorkspaceManager
{
    private readonly string _workspacesDirectory;
    private readonly ObservableCollection<Workspace> _workspaces = new();

    public IReadOnlyCollection<Workspace> Workspaces => _workspaces;

    public WorkspaceManager(string workspacesDirectory = "Workspaces")
    {
        if (Path.IsPathRooted(workspacesDirectory))
        {
            _workspacesDirectory = workspacesDirectory;
        }
        else
        {
            _workspacesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, workspacesDirectory);
        }

        if (!Directory.Exists(_workspacesDirectory))
        {
            Directory.CreateDirectory(_workspacesDirectory);
        }
        
        LoadWorkspaces();
    }

    private void LoadWorkspaces()
    {
        _workspaces.Clear();

        var workspaceFiles = Directory.GetFiles(_workspacesDirectory, "*.json");

        foreach (var file in workspaceFiles)
        {
            try
            {
                var json = File.ReadAllText(file);
                var workspace = JsonSerializer.Deserialize<Workspace>(json);
                if (workspace != null)
                {
                    _workspaces.Add(workspace);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading workspace {file}: {ex.Message}");
            }
        }

        // Sort by modified date (newest first)
        var sortedWorkspaces = _workspaces.OrderByDescending(w => w.Modified).ToList();
        _workspaces.Clear();
        foreach (var workspace in sortedWorkspaces)
        {
            _workspaces.Add(workspace);
        }
    }

    public async Task SaveWorkspaceAsync(Workspace workspace)
    {
        workspace.Modified = DateTime.Now;

        var fileName = $"{workspace.Name.Replace(" ", "_").Replace("/", "_")}.json";
        var filePath = Path.Combine(_workspacesDirectory, fileName);

        var json = JsonSerializer.Serialize(workspace, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(filePath, json);

        // Update or add to collection
        var existing = _workspaces.FirstOrDefault(w => w.Name == workspace.Name);
        if (existing != null)
        {
            var index = _workspaces.IndexOf(existing);
            _workspaces[index] = workspace;
        }
        else
        {
            _workspaces.Insert(0, workspace); // Add to beginning
        }
    }

    public void DeleteWorkspace(string workspaceName)
    {
        var fileName = $"{workspaceName.Replace(" ", "_").Replace("/", "_")}.json";
        var filePath = Path.Combine(_workspacesDirectory, fileName);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var workspace = _workspaces.FirstOrDefault(w => w.Name == workspaceName);
            if (workspace != null)
            {
                _workspaces.Remove(workspace);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting workspace: {ex.Message}");
        }
    }

    public Workspace? GetWorkspace(string name)
    {
        return _workspaces.FirstOrDefault(w => w.Name == name);
    }
}