using System.Windows;
using SilvaShell.App.Core;

namespace SilvaShell.App.Views;

public partial class TerminalCommandWindow : Window
{
    private readonly AppModule _app;

    public TerminalCommandWindow(AppModule app)
    {
        InitializeComponent();
        _app = app;
        AppNameText.Text = $"App: {_app.Name}";
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void Run_Click(object sender, RoutedEventArgs e)
    {
        var cmd = CommandText.Text?.Trim();
        if (string.IsNullOrWhiteSpace(cmd))
        {
            MessageBox.Show("Enter a command.");
            return;
        }

        OutputText.Text = "Running...";
        var result = await TerminalBridge.RunPowerShellAsync(cmd);

        OutputText.Text =
            $"ExitCode: {result.ExitCode}\n\nSTDOUT:\n{result.StdOut}\n\nSTDERR:\n{result.StdErr}";
    }
}
