using System.Collections.Generic;

namespace SilvaShell.App.Workspaces;

public class WorkspaceTab
{
    public string AppId { get; set; } = "";
    public string Url { get; set; } = "";
    public bool IsActive { get; set; }
}

public class Workspace
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public List<WorkspaceTab> Tabs { get; set; } = new();
    public string SidebarState { get; set; } = "expanded"; // expanded/collapsed
    public int ActiveTabIndex { get; set; }
}