using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace SilvaShell.App.Themes;

public enum ThemeType
{
    Light,
    Dark,
    Custom
}

public class Theme : INotifyPropertyChanged
{
    private string _name = "";
    private ThemeType _type;
    private Color _primaryColor = Colors.Blue;
    private Color _secondaryColor = Colors.LightBlue;
    private Color _backgroundColor = Colors.White;
    private Color _foregroundColor = Colors.Black;
    private Color _accentColor = Colors.Orange;
    private Color _sidebarBackground = Color.FromRgb(17, 17, 17);
    private Color _sidebarForeground = Colors.White;

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public ThemeType Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    public Color PrimaryColor
    {
        get => _primaryColor;
        set { _primaryColor = value; OnPropertyChanged(); OnPropertyChanged(nameof(PrimaryBrush)); }
    }

    public Color SecondaryColor
    {
        get => _secondaryColor;
        set { _secondaryColor = value; OnPropertyChanged(); OnPropertyChanged(nameof(SecondaryBrush)); }
    }

    public Color BackgroundColor
    {
        get => _backgroundColor;
        set { _backgroundColor = value; OnPropertyChanged(); OnPropertyChanged(nameof(BackgroundBrush)); }
    }

    public Color ForegroundColor
    {
        get => _foregroundColor;
        set { _foregroundColor = value; OnPropertyChanged(); OnPropertyChanged(nameof(ForegroundBrush)); }
    }

    public Color AccentColor
    {
        get => _accentColor;
        set { _accentColor = value; OnPropertyChanged(); OnPropertyChanged(nameof(AccentBrush)); }
    }

    public Color SidebarBackground
    {
        get => _sidebarBackground;
        set { _sidebarBackground = value; OnPropertyChanged(); OnPropertyChanged(nameof(SidebarBackgroundBrush)); }
    }

    public Color SidebarForeground
    {
        get => _sidebarForeground;
        set { _sidebarForeground = value; OnPropertyChanged(); OnPropertyChanged(nameof(SidebarForegroundBrush)); }
    }

    // Brush properties for XAML binding
    public SolidColorBrush PrimaryBrush => new SolidColorBrush(PrimaryColor);
    public SolidColorBrush SecondaryBrush => new SolidColorBrush(SecondaryColor);
    public SolidColorBrush BackgroundBrush => new SolidColorBrush(BackgroundColor);
    public SolidColorBrush ForegroundBrush => new SolidColorBrush(ForegroundColor);
    public SolidColorBrush AccentBrush => new SolidColorBrush(AccentColor);
    public SolidColorBrush SidebarBackgroundBrush => new SolidColorBrush(SidebarBackground);
    public SolidColorBrush SidebarForegroundBrush => new SolidColorBrush(SidebarForeground);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Predefined themes
    public static Theme LightTheme => new Theme
    {
        Name = "Light",
        Type = ThemeType.Light,
        PrimaryColor = Colors.Blue,
        SecondaryColor = Colors.LightBlue,
        BackgroundColor = Colors.White,
        ForegroundColor = Colors.Black,
        AccentColor = Colors.Orange,
        SidebarBackground = Color.FromRgb(240, 240, 240),
        SidebarForeground = Colors.Black
    };

    public static Theme DarkTheme => new Theme
    {
        Name = "Dark",
        Type = ThemeType.Dark,
        PrimaryColor = Colors.Cyan,
        SecondaryColor = Color.FromRgb(0, 100, 100),
        BackgroundColor = Color.FromRgb(30, 30, 30),
        ForegroundColor = Colors.White,
        AccentColor = Colors.Orange,
        SidebarBackground = Color.FromRgb(45, 45, 45),
        SidebarForeground = Colors.White
    };

    public static Theme BlueTheme => new Theme
    {
        Name = "Blue Ocean",
        Type = ThemeType.Custom,
        PrimaryColor = Color.FromRgb(0, 123, 255),
        SecondaryColor = Color.FromRgb(0, 86, 179),
        BackgroundColor = Color.FromRgb(248, 249, 250),
        ForegroundColor = Color.FromRgb(33, 37, 41),
        AccentColor = Color.FromRgb(255, 193, 7),
        SidebarBackground = Color.FromRgb(0, 123, 255),
        SidebarForeground = Colors.White
    };

    public static Theme ForestTheme => new Theme
    {
        Name = "Forest Green",
        Type = ThemeType.Custom,
        PrimaryColor = Color.FromRgb(34, 139, 34),
        SecondaryColor = Color.FromRgb(107, 142, 35),
        BackgroundColor = Color.FromRgb(240, 255, 240),
        ForegroundColor = Color.FromRgb(34, 139, 34),
        AccentColor = Color.FromRgb(255, 215, 0),
        SidebarBackground = Color.FromRgb(34, 139, 34),
        SidebarForeground = Colors.White
    };
}