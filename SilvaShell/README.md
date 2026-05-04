# SilvaShell - Windows AI & Communication Desktop

**SilvaShell** is a comprehensive Windows desktop application that serves as your daily cockpit for AI tools and communication. Built with WPF and WebView2, it wraps free web applications in a clean, organized interface with no APIs or local models required.

## 🌟 Features

### 🎯 Core Functionality
- **Web App Wrapper**: Host multiple AI and communication services in a single desktop window
- **Grouped Navigation**: Organized sidebar with AI, Email, and Chat categories
- **Favourites Bar**: Quick access to your most-used apps (Outlook, WhatsApp)
- **Collapsible Sidebar**: Toggle between full and minimal views
- **Hot-swappable Config**: Add/remove/edit apps without recompiling

### 🤖 AI Apps Included
- **DeepSeek**: Powerful reasoning and coding assistant
- **Qwen Chat & Studio**: Alibaba's multilingual AI models
- **LMArena**: Compare DeepSeek, Qwen, and Grok in one interface
- **MiniMax & Kimi**: Chinese AI stack

### 📧 Communication Apps
- **Outlook (Personal & Work)**: Full email management with folders, flags, and favourites
- **WhatsApp Web**: Complete chat interface with pinned conversations

### 🛠️ Advanced Features
- **Text-to-Speech**: "Speak Selection" button with voice support for:
  - Tamil (Jaffna / India)
  - English (UK)
  - English (US)
- **Terminal Bridge**: Optional PowerShell execution with approval
- **Browser Controls**: Back, Forward, Refresh navigation
- **Custom App Adding**: "+ Add App" button for any URL
- **Keyboard Shortcuts**: Ctrl+1 through Ctrl+9 for quick app switching
- **Persistent Login**: WebView2 profiles save Google and service logins

## 🚀 Getting Started

### Prerequisites
- Windows 10 or later
- .NET 8.0 SDK
- WebView2 Runtime (usually pre-installed on Windows)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/silva2kand/alldeskapp1-silvashell.git
   cd alldeskapp1-silvashell
   ```

2. **Build and run:**
   ```bash
   cd src/SilvaShell.App
   dotnet restore
   dotnet build
   dotnet run
   ```

3. **Create desktop shortcut (optional):**
   ```powershell
   $WshShell = New-Object -ComObject WScript.Shell
   $Shortcut = $WshShell.CreateShortcut("$env:USERPROFILE\Desktop\SilvaShell.lnk")
   $Shortcut.TargetPath = "PATH\TO\SilvaShell.exe"
   $Shortcut.WorkingDirectory = "PATH\TO\SilvaShell"
   $Shortcut.Save()
   ```

## 🎮 Usage

### Navigation
- **Sidebar Groups**: Click app names to switch between services
- **Favourites**: Starred apps at the top for instant access
- **Keyboard**: Ctrl+1..9 to jump directly to apps
- **Browser Controls**: Use Back/Forward/Refresh buttons

### Customization
- **Add Apps**: Click "+ Add App" to add any URL
- **Configure Apps**: Edit `src/SilvaShell.App/Config/apps.json`
- **Voice Settings**: Select from installed TTS voices in the dropdown

### TTS Usage
1. Select text on any webpage
2. Choose your preferred voice from the dropdown
3. Click "Speak Selection" to hear the text

## 🏗️ Architecture

### Project Structure
```
SilvaShell/
├── src/
│   └── SilvaShell.App/
│       ├── Core/                    # Business logic
│       │   ├── AppModule.cs        # App configuration model
│       │   ├── AppRegistry.cs      # App loading service
│       │   ├── SpeechService.cs    # TTS functionality
│       │   ├── TerminalBridge.cs   # PowerShell execution
│       │   └── HotkeyManager.cs    # Keyboard shortcuts
│       ├── Views/                  # WPF windows/dialogs
│       │   ├── AddAppWindow.xaml   # Custom app adder
│       │   └── TerminalCommandWindow.xaml # Terminal interface
│       ├── Config/                 # Configuration files
│       │   ├── apps.json          # App definitions
│       │   └── settings.json      # App settings
│       └── MainWindow.xaml        # Main application window
├── assets/                         # Icons and resources
└── SilvaShell.sln                 # Visual Studio solution
```

### Key Design Principles

1. **No External Dependencies**: Pure web wrapper - no APIs, no local models
2. **Modular Configuration**: Everything configurable via JSON
3. **Secure by Design**: Terminal commands require manual approval
4. **Persistent Sessions**: WebView2 profiles maintain login state
5. **Extensible**: Easy to add new apps and features

## 🔧 Configuration

### Adding New Apps

Edit `src/SilvaShell.App/Config/apps.json`:

```json
{
  "id": "my-app",
  "name": "My Custom App",
  "url": "https://example.com",
  "group": "Other",
  "allowTerminal": false,
  "requiresLogin": true,
  "regionSensitive": false,
  "languageFocus": "global"
}
```

Or use the built-in "+ Add App" button.

### Grouping Apps

Supported groups:
- `AI`: Artificial intelligence services
- `Email`: Email clients and services
- `Chat`: Messaging and communication apps
- `Other`: Custom or miscellaneous apps

## 🛡️ Security & Privacy

- **Isolated Sessions**: Each app runs in its own WebView2 instance
- **No Data Collection**: Pure client-side application
- **Approval-Gated Terminal**: Commands require explicit user approval
- **Persistent Profiles**: Login data stored locally only

## 📋 Requirements

### System Requirements
- Windows 10 version 1903 or later
- .NET 8.0 Runtime
- WebView2 Runtime
- 4GB RAM minimum (8GB recommended)

### TTS Voices
For full TTS functionality, install these Windows language packs:
- Tamil (India) - for Tamil (Jaffna) voice
- English (United Kingdom) - for English (UK) voice
- English (United States) - for English (US) voice

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Make your changes and test thoroughly
4. Commit your changes: `git commit -am 'Add new feature'`
5. Push to the branch: `git push origin feature/new-feature`
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with Microsoft's WebView2 and WPF
- Uses Windows built-in TTS for voice synthesis
- Inspired by the need for organized access to multiple AI and communication services

## 📞 Support

If you encounter issues:

1. Check the build requirements (.NET 8.0, WebView2)
2. Ensure no antivirus is blocking the application
3. Verify Windows language packs for TTS functionality
4. Check the GitHub Issues page for known problems

---

**SilvaShell** - Your complete AI and communication desktop cockpit. 🚀