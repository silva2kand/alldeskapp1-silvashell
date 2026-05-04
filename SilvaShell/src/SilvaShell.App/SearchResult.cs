using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SilvaShell.App;

public class SearchResult : INotifyPropertyChanged
{
    private string _tabTitle = "";
    private string _url = "";
    private string _snippet = "";
    private int _count = 0;
    private object? _tab;

    public string TabTitle
    {
        get => _tabTitle;
        set { _tabTitle = value; OnPropertyChanged(); }
    }

    public string Url
    {
        get => _url;
        set { _url = value; OnPropertyChanged(); }
    }

    public string Snippet
    {
        get => _snippet;
        set { _snippet = value; OnPropertyChanged(); }
    }

    public int Count
    {
        get => _count;
        set { _count = value; OnPropertyChanged(); }
    }

    public object? Tab
    {
        get => _tab;
        set { _tab = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}