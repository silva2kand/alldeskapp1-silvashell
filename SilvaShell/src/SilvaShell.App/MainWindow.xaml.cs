using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using SilvaShell.App.Core;
using SilvaShell.App.Notifications;
using SilvaShell.App.Plugins;
using SilvaShell.App.Themes;
using SilvaShell.App.Views;
using SilvaShell.App.Workspaces;

namespace SilvaShell.App;

public partial class MainWindow : Window
{
    private IReadOnlyList<AppModule> _apps = new List<AppModule>();
    private List<AppModule> _favourites = new();
    private bool _sidebarExpanded = true;
    private const double ExpandedWidth = 220;
    private const double CollapsedWidth = 50;
    private Dictionary<TabItem, AppTabData> _tabData = new();
    private PluginManager _pluginManager = new();
    private WorkspaceManager _workspaceManager = new();
    private NotificationManager _notificationManager = new();
    private ThemeManager _themeManager = new();
    private System.Timers.Timer? _autoSaveTimer;

    public MainWindow()
    {
        InitializeComponent();
        LoadApps();
        BuildFavourites();
        BuildSidebar();
        InitVoices();
        InitializePluginsAsync();
        InitializeAutoSave();
        InitializeTheme();
    }

    private async void InitializePluginsAsync()
    {
        try
        {
            await _pluginManager.LoadPluginsAsync();

            // Initialize plugin UI components
            InitializePluginUI();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading plugins: {ex.Message}");
        }
    }

    private void InitializePluginUI()
    {
        PluginToolbarArea.Children.Clear();

        // Add toolbar plugins to the main toolbar
        var toolbarPlugins = _pluginManager.GetPluginsOfType<IToolbarPlugin>();
        foreach (var plugin in toolbarPlugins)
        {
            try
            {
                var control = plugin.GetToolbarControl();
                if (control is UIElement uiElement)
                {
                    PluginToolbarArea.Children.Add(uiElement);
                }
                System.Diagnostics.Debug.WriteLine($"Loaded toolbar plugin: {plugin.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing toolbar plugin {plugin.Name}: {ex.Message}");
            }
        }

        // Add menu plugins
        var menuPlugins = _pluginManager.GetPluginsOfType<IMenuPlugin>();
        foreach (var plugin in menuPlugins)
        {
            try
            {
                var items = plugin.GetMenuItems();
                foreach (var item in items)
                {
                    PluginsMenu.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing menu plugin {plugin.Name}: {ex.Message}");
            }
        }

        // Register shortcuts
        var shortcutPlugins = _pluginManager.GetPluginsOfType<IShortcutPlugin>();
        foreach (var plugin in shortcutPlugins)
        {
            try
            {
                var shortcuts = plugin.GetShortcuts();
                foreach (var shortcut in shortcuts)
                {
                    // Register global shortcuts
                    var command = new RoutedCommand();
                    command.InputGestures.Add(shortcut.KeyGesture);
                    var binding = new CommandBinding(command, async (s, e) => await shortcut.Action());
                    this.CommandBindings.Add(binding);

                    System.Diagnostics.Debug.WriteLine($"Registered shortcut: {shortcut.Name} - {shortcut.KeyGesture}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing shortcut plugin {plugin.Name}: {ex.Message}");
            }
        }
    }

    private void InitializeAutoSave()
    {
        _autoSaveTimer = new System.Timers.Timer(300000); // 5 minutes
        _autoSaveTimer.Elapsed += async (s, e) => await AutoSaveWorkspaceAsync();
        _autoSaveTimer.Start();
    }

    private async Task AutoSaveWorkspaceAsync()
    {
        try
        {
            if (_tabData.Count > 0) // Only save if there are open tabs
            {
                var autoSaveWorkspace = CreateWorkspaceFromCurrentState("AutoSave");
                await _workspaceManager.SaveWorkspaceAsync(autoSaveWorkspace);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Auto-save failed: {ex.Message}");
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        // Save on exit
        _ = AutoSaveWorkspaceAsync();

        // Cleanup
        _autoSaveTimer?.Stop();
        _autoSaveTimer?.Dispose();

        base.OnClosed(e);
    }

    private void InitializeTheme()
    {
        // Apply default theme
        _themeManager.ApplyTheme(_themeManager.CurrentTheme);
    }

    // Notification handlers
    private void ViewNotificationsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var notificationWindow = new NotificationCenterWindow(_notificationManager);
        notificationWindow.Owner = this;
        notificationWindow.Show();
    }

    private void TestNotificationMenuItem_Click(object sender, RoutedEventArgs e)
    {
        // Add some test notifications
        _notificationManager.AddEmailNotification("john@example.com", "Project Update Meeting", "Outlook");
        _notificationManager.AddMessageNotification("Alice", "Hey, are you available for a call?", "WhatsApp");
        _notificationManager.AddUpdateNotification("DeepSeek", "New model version available");
        _notificationManager.AddNotification(new Notification("System", "Auto-save completed", NotificationType.Success));
    }

    private void MarkAllReadMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _notificationManager.MarkAllAsRead();
        MessageBox.Show("All notifications marked as read.", "Notifications",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ClearAllNotificationsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to clear all notifications?",
            "Clear Notifications", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _notificationManager.ClearAll();
        }
    }

    // Theme handlers
    private void LightThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _themeManager.ApplyTheme("Light");
    }

    private void DarkThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _themeManager.ApplyTheme("Dark");
    }

    private void BlueThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _themeManager.ApplyTheme("Blue Ocean");
    }

    private void ForestThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _themeManager.ApplyTheme("Forest Green");
    }

    private void CustomThemeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var themeName = Microsoft.VisualBasic.Interaction.InputBox(
            "Enter custom theme name:",
            "Create Custom Theme",
            "My Custom Theme");

        if (!string.IsNullOrWhiteSpace(themeName))
        {
            var customTheme = _themeManager.CreateCustomTheme(themeName);
            var themeEditor = new ThemeEditorWindow(customTheme, _themeManager);
            themeEditor.Owner = this;
            themeEditor.ShowDialog();
        }
    }

    private void ManageThemesMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var themeManagerWindow = new ThemeManagerWindow(_themeManager);
        themeManagerWindow.Owner = this;
        themeManagerWindow.ShowDialog();
    }

    private void LoadApps()
    {
        var configDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
        
        var configPath = Path.Combine(configDir, "apps.json");
        
        if (!File.Exists(configPath))
        {
            // If it doesn't exist in base directory, try to see if it's in a relative Config folder
            // (useful for development)
            var fallbackPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "apps.json");
            if (File.Exists(fallbackPath))
            {
                File.Copy(fallbackPath, configPath);
            }
            else
            {
                // Create a default apps.json if nothing found
                File.WriteAllText(configPath, "[]");
            }
        }
        
        _apps = AppRegistry.Load(configPath);
    }

    private void BuildFavourites()
    {
        _favourites = _apps.Take(5).ToList(); // Just take first 5 for now
    }

    private void BuildSidebar()
    {
        SidebarContentPanel.Children.Clear();

        // Add favourites
        var favHeader = new TextBlock
        {
            Text = "FAVOURITES",
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 10, 0, 5)
        };
        favHeader.SetResourceReference(TextBlock.ForegroundProperty, "ThemeSidebarForegroundBrush");
        favHeader.Opacity = 0.6;
        SidebarContentPanel.Children.Add(favHeader);

        foreach (var app in _favourites)
        {
            AddSidebarItem(app);
        }

        // Add all apps grouped
        var groups = _apps.GroupBy(a => a.Group);
        foreach (var group in groups)
        {
            var header = new TextBlock
            {
                Text = group.Key.ToUpper(),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 5)
            };
            header.SetResourceReference(TextBlock.ForegroundProperty, "ThemeSidebarForegroundBrush");
            header.Opacity = 0.6;
            SidebarContentPanel.Children.Add(header);

            foreach (var app in group)
            {
                AddSidebarItem(app);
            }
        }
    }

    private void AddSidebarItem(AppModule app)
    {
        var btn = new Button
        {
            Content = app.Name,
            Height = 35,
            Margin = new Thickness(0, 2, 0, 2),
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(10, 0, 0, 0),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Tag = app
        };
        btn.SetResourceReference(Control.ForegroundProperty, "ThemeSidebarForegroundBrush");

        btn.Click += (s, e) => OpenApp(app);
        SidebarContentPanel.Children.Add(btn);
    }

    private void OpenApp(AppModule app)
    {
        // Check if app already open
        var existingTab = _tabData.FirstOrDefault(t => t.Value.App.Id == app.Id).Key;
        if (existingTab != null)
        {
            MainTabControl.SelectedItem = existingTab;
            return;
        }

        var tabItem = new TabItem
        {
            Header = app.Name,
            Padding = new Thickness(10, 5, 10, 5)
        };

        var webView = new WebView2();
        var tabData = new AppTabData(app) { WebView = webView };
        _tabData[tabItem] = tabData;

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        // Toolbar
        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
        toolbar.SetResourceReference(StackPanel.BackgroundProperty, "ThemeSecondaryBrush");
        
        var backBtn = new Button { Content = "←", Width = 30, Margin = new Thickness(2) };
        backBtn.Click += (s, e) => { if (webView.CanGoBack) webView.GoBack(); };
        toolbar.Children.Add(backBtn);

        var forwardBtn = new Button { Content = "→", Width = 30, Margin = new Thickness(2) };
        forwardBtn.Click += (s, e) => { if (webView.CanGoForward) webView.GoForward(); };
        toolbar.Children.Add(forwardBtn);

        var refreshBtn = new Button { Content = "↻", Width = 30, Margin = new Thickness(2) };
        refreshBtn.Click += (s, e) => webView.Reload();
        toolbar.Children.Add(refreshBtn);

        var urlBar = new TextBox { Text = app.Url, Margin = new Thickness(5, 2, 5, 2), VerticalAlignment = VerticalAlignment.Center };
        urlBar.SetResourceReference(Control.BackgroundProperty, "ThemeBackgroundBrush");
        urlBar.SetResourceReference(Control.ForegroundProperty, "ThemeForegroundBrush");
        Grid.SetColumn(urlBar, 1);
        // In a real app we'd use a Grid for the toolbar to make urlBar stretch
        toolbar.Children.Add(urlBar);

        if (app.AllowTerminal)
        {
            var termBtn = new Button { Content = "Terminal", Margin = new Thickness(2), Padding = new Thickness(5, 0, 5, 0) };
            termBtn.Click += (s, e) => OpenTerminal(app);
            toolbar.Children.Add(termBtn);
        }

        grid.Children.Add(toolbar);
        Grid.SetRow(toolbar, 0);

        grid.Children.Add(webView);
        Grid.SetRow(webView, 1);

        tabItem.Content = grid;
        MainTabControl.Items.Add(tabItem);
        MainTabControl.SelectedItem = tabItem;

        webView.Source = new Uri(app.Url);
    }

    private void OpenTerminal(AppModule app)
    {
        var termWindow = new TerminalCommandWindow(app);
        termWindow.Owner = this;
        termWindow.Show();
    }

    private void InitVoices()
    {
        var voices = SpeechService.GetVoices();
        VoiceSelector.ItemsSource = voices;
        if (voices.Count > 0)
        {
            VoiceSelector.SelectedIndex = 0;
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            GlobalSearchBox.Focus();
            GlobalSearchBox.SelectAll();
            e.Handled = true;
        }
    }

    private void GlobalSearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ExecuteGlobalSearch();
        }
    }

    private void GlobalSearchBtn_Click(object sender, RoutedEventArgs e)
    {
        ExecuteGlobalSearch();
    }

    private async void ExecuteGlobalSearch()
    {
        var query = GlobalSearchBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(query)) return;

        var results = new ObservableCollection<SearchResult>();
        var searchTasks = new List<Task>();

        foreach (var entry in _tabData)
        {
            var tab = entry.Key;
            var data = entry.Value;
            var webView = data.WebView;

            if (webView == null) continue;

            searchTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        var script = @"
                            (function() {
                                var query = '" + query.Replace("'", "\\'") + @"';
                                var text = document.body.innerText;
                                var regex = new RegExp(query, 'gi');
                                var matches = text.match(regex);
                                var count = matches ? matches.length : 0;
                                var snippet = '';
                                if (count > 0) {
                                    var index = text.toLowerCase().indexOf(query.toLowerCase());
                                    var start = Math.max(0, index - 40);
                                    var end = Math.min(text.length, index + query.length + 40);
                                    snippet = '...' + text.substring(start, end).replace(/\n/g, ' ') + '...';
                                }
                                return JSON.stringify({ count: count, snippet: snippet });
                            })()";

                        var jsonResult = await webView.ExecuteScriptAsync(script);
                        if (jsonResult != null && jsonResult != "null")
                        {
                            // jsonResult is a JSON string from ExecuteScriptAsync, but since we return JSON.stringify, it's double-encoded
                            // WebView2 returns the result of the script as a JSON-encoded string.
                            var decodedJson = System.Text.Json.Nodes.JsonNode.Parse(jsonResult)?.ToString();
                            if (decodedJson != null)
                            {
                                var searchData = JsonSerializer.Deserialize<SearchData>(decodedJson);
                                if (searchData != null && searchData.count > 0)
                                {
                                    results.Add(new SearchResult
                                    {
                                        TabTitle = data.App.Name,
                                        Count = searchData.count,
                                        Snippet = searchData.snippet ?? "",
                                        Tab = tab
                                    });
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Search error in tab {data.App.Name}: {ex.Message}");
                }
            }));
        }

        await Task.WhenAll(searchTasks);

        if (results.Count > 0)
        {
            var resultsWindow = new SearchResultsWindow(query, results, this);
            resultsWindow.Owner = this;
            resultsWindow.Show();
        }
        else
        {
            MessageBox.Show($"No matches found for '{query}' in any open tabs.", "Search Results");
        }
    }

    private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private async void SaveWorkspaceMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var name = Microsoft.VisualBasic.Interaction.InputBox("Enter workspace name:", "Save Workspace", "New Workspace");
        if (!string.IsNullOrWhiteSpace(name))
        {
            var workspace = CreateWorkspaceFromCurrentState(name);
            await _workspaceManager.SaveWorkspaceAsync(workspace);
            MessageBox.Show($"Workspace '{name}' saved.");
        }
    }

    private void LoadWorkspaceMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var manageWindow = new WorkspaceManagerWindow(_workspaceManager, this);
        manageWindow.Owner = this;
        manageWindow.ShowDialog();
    }

    private void ManageWorkspacesMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var manageWindow = new WorkspaceManagerWindow(_workspaceManager, this);
        manageWindow.Owner = this;
        manageWindow.ShowDialog();
    }

    private void ManagePluginsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var pluginWindow = new PluginManagerWindow(_pluginManager);
        pluginWindow.Owner = this;
        pluginWindow.ShowDialog();
    }

    private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("SilvaShell v1.0\nAn Advanced AI & Communication Desktop", "About");
    }

    private void ToggleSidebarBtn_Click(object sender, RoutedEventArgs e)
    {
        _sidebarExpanded = !_sidebarExpanded;
        SidebarPanel.Width = _sidebarExpanded ? ExpandedWidth : CollapsedWidth;
        ToggleSidebarBtn.Content = _sidebarExpanded ? "◀" : "▶";
        
        // Hide/show text in sidebar buttons
        foreach (var child in SidebarContentPanel.Children)
        {
            if (child is Button btn)
            {
                if (!_sidebarExpanded)
                {
                    btn.Content = btn.Content.ToString()?.Substring(0, 1);
                }
                else
                {
                    if (btn.Tag is AppModule app)
                    {
                        btn.Content = app.Name;
                    }
                }
            }
        }
    }

    private void AddAppBtn_Click(object sender, RoutedEventArgs e)
    {
        var addAppWindow = new AddAppWindow();
        addAppWindow.Owner = this;
        if (addAppWindow.ShowDialog() == true)
        {
            // App added logic
            LoadApps();
            BuildSidebar();
        }
    }

    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Handle tab selection change
    }

    private void SpeakSelection_Click(object sender, RoutedEventArgs e)
    {
        if (MainTabControl.SelectedItem is TabItem tab && _tabData.TryGetValue(tab, out var data))
        {
            // In a real app we'd get selected text from WebView2
            // For now just speak the app name
            var selectedVoice = VoiceSelector.SelectedItem as TtsVoice;
            if (selectedVoice != null)
            {
                SpeechService.SetVoice(selectedVoice.Name);
            }
            SpeechService.SpeakAsync($"You are viewing {data.App.Name}");
        }
    }

    private void CloseTabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.TemplatedParent is TabItem tab)
        {
            _tabData.Remove(tab);
            MainTabControl.Items.Remove(tab);
        }
    }

    public Workspace CreateWorkspaceFromCurrentState(string name)
    {
        var workspace = new Workspace
        {
            Name = name,
            SidebarState = _sidebarExpanded ? "expanded" : "collapsed",
            ActiveTabIndex = MainTabControl.SelectedIndex
        };

        foreach (TabItem tab in MainTabControl.Items)
        {
            if (tab == WelcomeTab) continue;

            if (_tabData.TryGetValue(tab, out var data))
            {
                workspace.Tabs.Add(new WorkspaceTab
                {
                    AppId = data.App.Id,
                    Url = data.WebView.Source?.ToString() ?? data.App.Url,
                    IsActive = tab == MainTabControl.SelectedItem
                });
            }
        }

        return workspace;
    }

    public async Task LoadWorkspaceAsync(Workspace workspace)
    {
        // Clear existing tabs (except welcome)
        var tabsToRemove = MainTabControl.Items.Cast<TabItem>().Where(t => t != WelcomeTab).ToList();
        foreach (var tab in tabsToRemove)
        {
            _tabData.Remove(tab);
            MainTabControl.Items.Remove(tab);
        }

        // Set sidebar state
        _sidebarExpanded = workspace.SidebarState == "expanded";
        SidebarPanel.Width = _sidebarExpanded ? ExpandedWidth : CollapsedWidth;
        ToggleSidebarBtn.Content = _sidebarExpanded ? "◀" : "▶";

        // Load tabs
        foreach (var tabInfo in workspace.Tabs)
        {
            var app = _apps.FirstOrDefault(a => a.Id == tabInfo.AppId);
            if (app != null)
            {
                // Open app with specific URL
                var tabItem = new TabItem
                {
                    Header = app.Name,
                    Padding = new Thickness(10, 5, 10, 5)
                };

                var webView = new WebView2();
                var tabData = new AppTabData(app) { WebView = webView };
                _tabData[tabItem] = tabData;

                // Simple grid setup (similar to OpenApp)
                var grid = new Grid();
                grid.Children.Add(webView);
                tabItem.Content = grid;

                MainTabControl.Items.Add(tabItem);
                
                await webView.EnsureCoreWebView2Async();
                webView.Source = new Uri(tabInfo.Url);
            }
        }

        // Set active tab
        if (workspace.ActiveTabIndex >= 0 && workspace.ActiveTabIndex < MainTabControl.Items.Count)
        {
            MainTabControl.SelectedIndex = workspace.ActiveTabIndex;
        }
    }

    private class SearchData
    {
        public int count { get; set; }
        public string? snippet { get; set; }
    }
}
