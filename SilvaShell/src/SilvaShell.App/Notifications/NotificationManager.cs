using System.Collections.ObjectModel;
using System.Windows;

namespace SilvaShell.App.Notifications;

public class NotificationManager
{
    private readonly ObservableCollection<Notification> _notifications = new();
    private readonly ObservableCollection<Notification> _unreadNotifications = new();

    public IReadOnlyCollection<Notification> Notifications => _notifications;
    public IReadOnlyCollection<Notification> UnreadNotifications => _unreadNotifications;

    public event EventHandler<Notification>? NotificationAdded;

    public void AddNotification(Notification notification)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _notifications.Insert(0, notification); // Add to beginning
            _unreadNotifications.Insert(0, notification);

            NotificationAdded?.Invoke(this, notification);

            // Show toast notification
            ShowToastNotification(notification);
        });
    }

    public void MarkAsRead(string notificationId)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            _unreadNotifications.Remove(notification);
        }
    }

    public void MarkAllAsRead()
    {
        foreach (var notification in _unreadNotifications.ToList())
        {
            notification.IsRead = true;
            _unreadNotifications.Remove(notification);
        }
    }

    public void ClearAll()
    {
        _notifications.Clear();
        _unreadNotifications.Clear();
    }

    private void ShowToastNotification(Notification notification)
    {
        // For now, show a simple message box. In a full implementation,
        // this would show a proper toast notification
        var result = MessageBox.Show(
            $"{notification.Title}\n\n{notification.Message}",
            $"SilvaShell - {notification.SourceApp}",
            MessageBoxButton.OK,
            GetMessageBoxImage(notification.Type));

        if (result == MessageBoxResult.OK)
        {
            MarkAsRead(notification.Id);
        }
    }

    private MessageBoxImage GetMessageBoxImage(NotificationType type)
    {
        return type switch
        {
            NotificationType.Info => MessageBoxImage.Information,
            NotificationType.Success => MessageBoxImage.Information,
            NotificationType.Warning => MessageBoxImage.Warning,
            NotificationType.Error => MessageBoxImage.Error,
            NotificationType.Message => MessageBoxImage.Information,
            NotificationType.Update => MessageBoxImage.Information,
            _ => MessageBoxImage.Information
        };
    }

    // Helper methods for common notifications
    public void AddEmailNotification(string from, string subject, string appName = "Outlook")
    {
        var notification = new Notification(
            "New Email",
            $"From: {from}\nSubject: {subject}",
            NotificationType.Message)
        {
            SourceApp = appName
        };
        AddNotification(notification);
    }

    public void AddMessageNotification(string from, string message, string appName = "WhatsApp")
    {
        var notification = new Notification(
            "New Message",
            $"From: {from}\n{message}",
            NotificationType.Message)
        {
            SourceApp = appName
        };
        AddNotification(notification);
    }

    public void AddUpdateNotification(string appName, string updateInfo)
    {
        var notification = new Notification(
            "App Update",
            $"{appName}: {updateInfo}",
            NotificationType.Update)
        {
            SourceApp = appName
        };
        AddNotification(notification);
    }
}