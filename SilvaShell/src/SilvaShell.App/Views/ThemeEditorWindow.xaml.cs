using System.Windows;
using System.Windows.Media;
using SilvaShell.App.Themes;

namespace SilvaShell.App.Views;

public partial class ThemeEditorWindow : Window
{
    private readonly Theme _theme;
    private readonly ThemeManager _themeManager;

    public ThemeEditorWindow(Theme theme, ThemeManager themeManager)
    {
        InitializeComponent();
        _theme = theme;
        _themeManager = themeManager;

        Title = $"Theme Editor - {_theme.Name}";
        LoadThemeValues();
    }

    private void LoadThemeValues()
    {
        PrimaryColorTextBox.Text = _theme.PrimaryColor.ToString();
        SecondaryColorTextBox.Text = _theme.SecondaryColor.ToString();
        BackgroundColorTextBox.Text = _theme.BackgroundColor.ToString();
        ForegroundColorTextBox.Text = _theme.ForegroundColor.ToString();
        AccentColorTextBox.Text = _theme.AccentColor.ToString();
        SidebarBackgroundTextBox.Text = _theme.SidebarBackground.ToString();
        SidebarForegroundTextBox.Text = _theme.SidebarForeground.ToString();

        UpdatePreviews();
    }

    private void UpdatePreviews()
    {
        PrimaryColorPreview.Fill = new SolidColorBrush(_theme.PrimaryColor);
        SecondaryColorPreview.Fill = new SolidColorBrush(_theme.SecondaryColor);
        BackgroundColorPreview.Fill = new SolidColorBrush(_theme.BackgroundColor);
        ForegroundColorPreview.Fill = new SolidColorBrush(_theme.ForegroundColor);
        AccentColorPreview.Fill = new SolidColorBrush(_theme.AccentColor);
        SidebarBackgroundPreview.Fill = new SolidColorBrush(_theme.SidebarBackground);
        SidebarForegroundPreview.Fill = new SolidColorBrush(_theme.SidebarForeground);
    }

    private void UpdateThemeFromUI()
    {
        try
        {
            _theme.PrimaryColor = (Color)ColorConverter.ConvertFromString(PrimaryColorTextBox.Text);
            _theme.SecondaryColor = (Color)ColorConverter.ConvertFromString(SecondaryColorTextBox.Text);
            _theme.BackgroundColor = (Color)ColorConverter.ConvertFromString(BackgroundColorTextBox.Text);
            _theme.ForegroundColor = (Color)ColorConverter.ConvertFromString(ForegroundColorTextBox.Text);
            _theme.AccentColor = (Color)ColorConverter.ConvertFromString(AccentColorTextBox.Text);
            _theme.SidebarBackground = (Color)ColorConverter.ConvertFromString(SidebarBackgroundTextBox.Text);
            _theme.SidebarForeground = (Color)ColorConverter.ConvertFromString(SidebarForegroundTextBox.Text);
        }
        catch
        {
            MessageBox.Show("Invalid color format. Use format like #FF0000 or Red.");
        }
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        UpdateThemeFromUI();
        _themeManager.ApplyTheme(_theme);
        UpdatePreviews();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        UpdateThemeFromUI();

        try
        {
            await _themeManager.SaveThemeAsync(_theme);
            _themeManager.ApplyTheme(_theme);
            MessageBox.Show($"Theme '{_theme.Name}' saved successfully!", "Theme Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving theme: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}