using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SilvaShell.App.Themes;

public class ThemeManager
{
    private readonly string _themesDirectory;
    private readonly ObservableCollection<Theme> _availableThemes = new();
    private Theme _currentTheme;

    public IReadOnlyCollection<Theme> AvailableThemes => _availableThemes;
    public Theme CurrentTheme
    {
        get => _currentTheme;
        private set
        {
            _currentTheme = value;
            ApplyTheme(value);
        }
    }

    public event EventHandler<Theme>? ThemeChanged;

    public ThemeManager(string themesDirectory = "Themes")
    {
        if (Path.IsPathRooted(themesDirectory))
        {
            _themesDirectory = themesDirectory;
        }
        else
        {
            _themesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, themesDirectory);
        }

        if (!Directory.Exists(_themesDirectory))
        {
            Directory.CreateDirectory(_themesDirectory);
        }

        // Load built-in themes
        LoadBuiltInThemes();

        // Load custom themes
        LoadCustomThemes();

        // Set default theme
        _currentTheme = AvailableThemes.FirstOrDefault(t => t.Type == ThemeType.Light) ?? AvailableThemes.First();
    }

    private void LoadBuiltInThemes()
    {
        _availableThemes.Add(Theme.LightTheme);
        _availableThemes.Add(Theme.DarkTheme);
        _availableThemes.Add(Theme.BlueTheme);
        _availableThemes.Add(Theme.ForestTheme);
    }

    private void LoadCustomThemes()
    {
        var themeFiles = Directory.GetFiles(_themesDirectory, "*.json");

        foreach (var file in themeFiles)
        {
            try
            {
                var json = File.ReadAllText(file);
                var theme = JsonSerializer.Deserialize<Theme>(json);
                if (theme != null)
                {
                    theme.Type = ThemeType.Custom;
                    _availableThemes.Add(theme);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading theme {file}: {ex.Message}");
            }
        }
    }

    public async Task SaveThemeAsync(Theme theme)
    {
        if (theme.Type != ThemeType.Custom)
            return; // Only save custom themes

        var fileName = $"{theme.Name.Replace(" ", "_").Replace("/", "_")}.json";
        var filePath = Path.Combine(_themesDirectory, fileName);

        var json = JsonSerializer.Serialize(theme, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(filePath, json);

        // Add to available themes if not already present
        if (!_availableThemes.Contains(theme))
        {
            _availableThemes.Add(theme);
        }
    }

    public void ApplyTheme(string themeName)
    {
        var theme = _availableThemes.FirstOrDefault(t => t.Name == themeName);
        if (theme != null)
        {
            ApplyTheme(theme);
        }
    }

    public void ApplyTheme(Theme theme)
    {
        _currentTheme = theme;

        // Apply theme to application resources
        var app = System.Windows.Application.Current;
        if (app != null)
        {
            app.Resources["ThemePrimaryBrush"] = theme.PrimaryBrush;
            app.Resources["ThemeSecondaryBrush"] = theme.SecondaryBrush;
            app.Resources["ThemeBackgroundBrush"] = theme.BackgroundBrush;
            app.Resources["ThemeForegroundBrush"] = theme.ForegroundBrush;
            app.Resources["ThemeAccentBrush"] = theme.AccentBrush;
            app.Resources["ThemeSidebarBackgroundBrush"] = theme.SidebarBackgroundBrush;
            app.Resources["ThemeSidebarForegroundBrush"] = theme.SidebarForegroundBrush;

            // Update window backgrounds, etc.
            UpdateWindowThemes(theme);
        }

        ThemeChanged?.Invoke(this, theme);
    }

    private void UpdateWindowThemes(Theme theme)
    {
        // This would update all open windows with the new theme
        // For now, we'll rely on data binding in XAML
    }

    public Theme CreateCustomTheme(string name)
    {
        return new Theme
        {
            Name = name,
            Type = ThemeType.Custom,
            PrimaryColor = Colors.Purple,
            SecondaryColor = Colors.MediumPurple,
            BackgroundColor = Colors.White,
            ForegroundColor = Colors.Black,
            AccentColor = Colors.Gold,
            SidebarBackground = Colors.Purple,
            SidebarForeground = Colors.White
        };
    }

    public void DeleteCustomTheme(Theme theme)
    {
        if (theme.Type != ThemeType.Custom)
            return;

        var fileName = $"{theme.Name.Replace(" ", "_").Replace("/", "_")}.json";
        var filePath = Path.Combine(_themesDirectory, fileName);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _availableThemes.Remove(theme);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting theme: {ex.Message}");
        }
    }
}