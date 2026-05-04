using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SilvaShell.App.Views;

public partial class SearchResultsWindow : Window
{
    private readonly ObservableCollection<SearchResult> _results;
    private readonly string _query;
    private readonly MainWindow _mainWindow;

    public SearchResultsWindow(string query, ObservableCollection<SearchResult> results, MainWindow mainWindow)
    {
        InitializeComponent();
        _query = query;
        _results = results;
        _mainWindow = mainWindow;

        ResultsHeader.Text = $"Search Results for '{query}' ({results.Count} tabs with matches)";
        ResultsListView.ItemsSource = results;
    }

    private void ResultsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ResultsListView.SelectedItem is SearchResult selectedResult)
        {
            if (selectedResult.Tab is TabItem tab)
            {
                _mainWindow.MainTabControl.SelectedItem = tab;
                Close();
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}