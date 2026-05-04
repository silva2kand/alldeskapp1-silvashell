using System.Windows;
using System.Windows.Controls;
using SilvaShell.App.Themes;

namespace SilvaShell.App.Views;

public partial class ThemeManagerWindow : Window
{
    private readonly ThemeManager _themeManager;

    public ThemeManagerWindow(ThemeManager themeManager)
    {
        InitializeComponent();
        _themeManager = themeManager;
        LoadThemes();
    }

    private void LoadThemes()
    {
        var themesWithCurrent = _themeManager.AvailableThemes.Select(t => new
        {
            Theme = t,
            Name = t.Name,
            Type = t.Type.ToString(),
            IsCurrent = t == _themeManager.CurrentTheme
        });

        ThemesListView.ItemsSource = themesWithCurrent;
    }

    private void ApplyThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = ThemesListView.SelectedItem;
        if (selectedItem != null)
        {
            var theme = ((dynamic)selectedItem).Theme as Theme;
            if (theme != null)
            {
                _themeManager.ApplyTheme(theme);
                LoadThemes(); // Refresh to show current theme
                MessageBox.Show($"Theme '{theme.Name}' applied successfully!", "Theme Applied", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        else
        {
            MessageBox.Show("Please select a theme to apply.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void EditThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = ThemesListView.SelectedItem;
        if (selectedItem != null)
        {
            var theme = ((dynamic)selectedItem).Theme as Theme;
            if (theme != null)
            {
                if (theme.Type != ThemeType.Custom)
                {
                    var result = MessageBox.Show(
                        "This is a built-in theme. Create a custom copy to edit?",
                        "Edit Theme",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var customTheme = _themeManager.CreateCustomTheme($"{theme.Name} (Custom)");
                        customTheme.PrimaryColor = theme.PrimaryColor;
                        customTheme.SecondaryColor = theme.SecondaryColor;
                        customTheme.BackgroundColor = theme.BackgroundColor;
                        customTheme.ForegroundColor = theme.ForegroundColor;
                        customTheme.AccentColor = theme.AccentColor;
                        customTheme.SidebarBackground = theme.SidebarBackground;
                        customTheme.SidebarForeground = theme.SidebarForeground;

                        var editor = new ThemeEditorWindow(customTheme, _themeManager);
                        editor.Owner = this;
                        editor.ShowDialog();
                        LoadThemes();
                    }
                }
                else
                {
                    var editor = new ThemeEditorWindow(theme, _themeManager);
                    editor.Owner = this;
                    editor.ShowDialog();
                    LoadThemes();
                }
            }
        }
        else
        {
            MessageBox.Show("Please select a theme to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void DeleteThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = ThemesListView.SelectedItem;
        if (selectedItem != null)
        {
            var theme = ((dynamic)selectedItem).Theme as Theme;
            if (theme != null)
            {
                if (theme.Type != ThemeType.Custom)
                {
                    MessageBox.Show("Built-in themes cannot be deleted.", "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete theme '{theme.Name}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _themeManager.DeleteCustomTheme(theme);
                    LoadThemes();
                }
            }
        }
        else
        {
            MessageBox.Show("Please select a theme to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

// Value converter for boolean to visibility
public class BooleanToVisibilityConverter : System.Windows.Data.IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (bool)value ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new System.NotImplementedException();
    }
}