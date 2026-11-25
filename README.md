# WordOverlay

WordOverlay is a lightweight, C# WPF desktop application designed to passively expand your vocabulary. Upon startup, it renders a semi-transparent, resizable overlay pinned to the top-right corner of your desktop. Rather than floating over active applications, the window is sent to the background layer, keeping it visible on your wallpaper without interrupting your workflow.

## Key Features

* **Desktop-Level Integration:** Pinned to the desktop background (`HWND_BOTTOM`) so it never obstructs active windows or dialogs.
* **Subtle & Customizable:** Displays a mostly transparent widget in the top-right corner with adjustable dimensions to fit your screen resolution and aesthetic preferences.
* **Automated Rotation:** Periodically cycles through advanced vocabulary words and their definitions.
* **Extensible Dictionary:** Powered by a local `words.json` file, allowing you to easily add, remove, or modify word entries.
* **Run on Startup:** Configured to initialize automatically when Windows starts.

## Project Structure

```text
WordOverlay/
├── Models/
│   └── WordEntry.cs          # Data model representing a vocabulary word and its definition
├── Resources/
│   └── words.json            # JSON dataset containing difficult words and definitions
├── Services/
│   ├── ConfigService.cs      # Manages window dimensions, opacity, rotation intervals, and startup registry settings
│   └── WordServices.cs       # Handles loading, parsing, and cycling through the vocabulary dataset
├── .gitignore                # Standard Visual Studio / .NET git ignore rules
├── App.xaml                  # Application-level resources and global styles
├── App.xaml.cs               # Application entry point and startup lifecycle logic
├── OverlayWindow.xaml        # UI layout and transparency styling for the desktop widget
├── OverlayWindow.xaml.cs     # Window positioning, resizing logic, and Win32 z-order (send-to-back) handling
└── WordOverlay.csproj        # Project configuration and dependencies
```

## Prerequisites

* **OS:** Windows 10 or Windows 11
* **SDK:** [.NET 6.0 / .NET 8.0 SDK](https://dotnet.microsoft.com/download) (or the target framework specified in `WordOverlay.csproj`)
* **IDE (Optional):** Visual Studio 2022 or JetBrains Rider

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/WordOverlay.git
cd WordOverlay
```

### 2. Build the Project
Using the .NET CLI:
```bash
dotnet build --configuration Release
```
Alternatively, open `WordOverlay.csproj` in Visual Studio and build the solution (`Ctrl + Shift + B`).

### 3. Run the Application
```bash
dotnet run --project WordOverlay.csproj
```

## Configuration & Customization

### Adding Vocabulary (`Resources/words.json`)
You can expand the vocabulary list by editing `Resources/words.json`. Ensure the file follows the structure expected by `WordEntry.cs`:

```json
[
  {
    "word": "Ephemeral",
    "definition": "Lasting for a very short time."
  },
  {
    "word": "Obfuscate",
    "definition": "To render obscure, unclear, or unintelligible."
  }
]
```

### Adjusting Window Size & Behavior
* **Resizing:** Drag the edges/corners of the overlay (when unlocked/hovered, depending on your `OverlayWindow` implementation) or adjust the default width and height values managed by `ConfigService.cs`.
* **Startup Behavior:** The application registers itself in the Windows Startup registry (`HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`) via `ConfigService.cs` so it launches seamlessly on login.

## License

This project is open-source and available under the [MIT License](LICENSE).