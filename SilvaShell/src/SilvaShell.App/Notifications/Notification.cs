using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SilvaShell.App.Notifications;

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    Message,
    Update
}

public class Notification : INotifyPropertyChanged
{
    private string _title = "";
    private string _message = "";
    private NotificationType _type;
    private DateTime _timestamp;
    private bool _isRead;
    private string? _actionUrl;

    public string Id { get; } = Guid.NewGuid().ToString();

    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    public NotificationType Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    public DateTime Timestamp
    {
        get => _timestamp;
        set { _timestamp = value; OnPropertyChanged(); }
    }

    public bool IsRead
    {
        get => _isRead;
        set { _isRead = value; OnPropertyChanged(); }
    }

    public string? ActionUrl
    {
        get => _actionUrl;
        set { _actionUrl = value; OnPropertyChanged(); }
    }

    public string SourceApp { get; set; } = "";

    public Notification(string title, string message, NotificationType type = NotificationType.Info)
    {
        Title = title;
        Message = message;
        Type = type;
        Timestamp = DateTime.Now;
        IsRead = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}