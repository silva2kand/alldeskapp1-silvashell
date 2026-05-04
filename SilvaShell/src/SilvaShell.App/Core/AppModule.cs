namespace SilvaShell.App.Core;

public class AppModule
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Group { get; set; } = "Other";
    public bool AllowTerminal { get; set; }

    // Extra metadata
    public bool RequiresLogin { get; set; }
    public bool RegionSensitive { get; set; }
    public string LanguageFocus { get; set; } = "global";
    public string Notes { get; set; } = "";
}
