using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SilvaShell.App.Notifications;

namespace SilvaShell.App.Views;

public partial class NotificationCenterWindow : Window
{
    private readonly NotificationManager _notificationManager;

    public NotificationCenterWindow(NotificationManager notificationManager)
    {
        InitializeComponent();
        _notificationManager = notificationManager;

        // Set up data binding
        NotificationsListView.ItemsSource = _notificationManager.Notifications;

        // Update unread count
        UpdateUnreadCount();

        // Listen for new notifications
        _notificationManager.NotificationAdded += (s, e) => UpdateUnreadCount();
    }

    private void UpdateUnreadCount()
    {
        var unreadCount = _notificationManager.UnreadNotifications.Count;
        UnreadCountText.Text = unreadCount > 0 ? $" ({unreadCount} unread)" : "";
    }

    private void MarkReadButton_Click(object sender, RoutedEventArgs e)
    {
        if (NotificationsListView.SelectedItem is Notification selectedNotification)
        {
            _notificationManager.MarkAsRead(selectedNotification.Id);
            UpdateUnreadCount();
        }
    }

    private void MarkAllReadButton_Click(object sender, RoutedEventArgs e)
    {
        _notificationManager.MarkAllAsRead();
        UpdateUnreadCount();
    }

    private void ClearAllButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to clear all notifications?",
            "Clear Notifications", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _notificationManager.ClearAll();
            UpdateUnreadCount();
        }
    }
}

// Value converters for the notification UI
public class BoolToBackgroundConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (bool)value ? Brushes.White : Brushes.LightBlue;
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new System.NotImplementedException();
    }
}

public class NotificationTypeToColorConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var type = (NotificationType)value;
        return type switch
        {
            NotificationType.Info => Brushes.Blue,
            NotificationType.Success => Brushes.Green,
            NotificationType.Warning => Brushes.Orange,
            NotificationType.Error => Brushes.Red,
            NotificationType.Message => Brushes.Purple,
            NotificationType.Update => Brushes.Teal,
            _ => Brushes.Black
        };
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new System.NotImplementedException();
    }
}