using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;
using SilvaShell.App.Core;

namespace SilvaShell.App;

public class AppTabData
{
    public AppModule App { get; set; } = new();
    public WebView2 WebView { get; set; } = new();
    public Button TerminalButton { get; set; } = new();
    public Button BackButton { get; set; } = new();
    public Button ForwardButton { get; set; } = new();
    public Button RefreshButton { get; set; } = new();

    public AppTabData(AppModule app)
    {
        App = app;
    }
}