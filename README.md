# Tag Analyzer - .NET 10 + C# 14

A cross-platform desktop application based on .NET 10 and Avalonia UI, using C# 14 latest syntax features, designed to analyze tag information in text files.

[中文文档](README.zh-CN.md)

## Features

- 🎯 **Smart Parsing**: Automatically recognizes `【tag】` format in text
- 📊 **Statistical Analysis**: Counts occurrences and source IDs for each tag
- 🖥️ **Modern UI**: Modern graphical interface based on Avalonia UI
- 📁 **File Selection**: Supports selecting text files through dialog
- 📄 **CSV Export**: Supports exporting analysis results to CSV format
- 🔍 **ID Tracking**: Records specific ID locations for each tag
- 🔢 **Format Compatibility**: Supports all M-prefixed number formats (M24, M35, M1, M100, etc.)
- 🌐 **Cross-platform**: Supports Windows, Linux, macOS
- 📋 **Version Info**: Displays version information and About window

## System Requirements

- .NET 10.0 Runtime or SDK
- Windows 10/11, Linux, or macOS
- C# 14 syntax features support

## Quick Start

### 1. Download Pre-built Version (Recommended)

📦 **GitHub Releases**: Visit the [Releases page](https://github.com/JiashuaiXu/TagAnalyzer-/releases) to download the latest pre-built executable.

- Extract the downloaded ZIP file
- Double-click `TagAnalyzer.exe` to run
- No .NET Runtime installation required (self-contained release)

### 2. Build from Source

```bash
# Clone or download the project
# Navigate to project directory

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

### 3. Manual Publishing

```powershell
# Windows PowerShell
.\publish.ps1
```

After publishing, the executable will be located at `./publish/TagAnalyzer.exe`

## Usage

1. **Launch the application**: Double-click `TagAnalyzer.exe` or run `dotnet run`
2. **Select file**: Click "Select File" button to choose a `.txt` file
3. **View results**: The application will automatically parse and display tag statistics
4. **Export data**: Click "Export CSV" button to save results to file
5. **Clear results**: Click "Clear Results" button to start over

## Supported Text Format

The application is specifically designed to process text files in the following format:

```
M24_230001【tag】Some text content
	Phonetic line (starting with tab, will be ignored)
M35_230002【sigh】【tag】Another line of content
	More phonetic content
M100_230003【other tag】Third line content
```

### Parsing Rules

- ✅ **ID Line**: Lines starting with `M` followed by number and 6-digit ID will be processed (e.g., `M24_230001`, `M35_230002`, `M100_230003`)
- ❌ **Phonetic Line**: Lines starting with tab will be ignored
- 🏷️ **Tag Format**: Text in `【tag content】` format will be extracted
- 📝 **Multiple Tags**: A single line can contain multiple tags
- 🔢 **Number Format**: Supports all `M` + number + `_` + 6-digit format (e.g., M1, M24, M35, M100)

## Output Format

### UI Display
| Tag | Count | Source IDs |
|------|-------|------------|
| tag | 2 | M24_230001, M35_230002 |
| sigh | 1 | M35_230002 |

### CSV Export
```csv
Tag,Count,Source IDs
tag,2,"M24_230001, M35_230002"
sigh,1,M35_230002
```

## Tech Stack

- **Framework**: .NET 10.0
- **Language**: C# 14 (latest syntax features)
- **UI Framework**: Avalonia UI 11.1
- **Architecture**: MVVM
- **CSV Processing**: CsvHelper
- **Regular Expressions**: System.Text.RegularExpressions

## Project Structure

```
TagAnalyzer/
├── Models/
│   ├── TextParser.cs          # Text parsing logic
│   └── VersionInfo.cs         # Version information utility
├── ViewModels/
│   └── MainWindowViewModel.cs  # Main window view model
├── MainWindow.axaml           # Main window UI
├── MainWindow.axaml.cs        # Main window code-behind
├── AboutWindow.axaml          # About window UI
├── AboutWindow.axaml.cs       # About window code-behind
├── App.axaml                  # Application definition
├── App.axaml.cs              # Application code-behind
├── Program.cs                # Program entry point
├── TagAnalyzer.csproj        # Project file
├── publish.ps1              # Publish script
└── README.md                # Documentation (English)
```

## Development Info

- **Author**: jiashuai_xu@qq.com
- **Version**: 1.1.0
- **License**: MIT
- **Development Environment**: Visual Studio 2022 / VS Code
- **Target Framework**: .NET 10.0
- **Language Version**: C# 14

## FAQ

### Q: Application won't start?
A: Make sure you have .NET 10.0 Runtime installed, or use `dotnet run` command.

### Q: Can't select files?
A: Make sure the file format is `.txt`, the application will automatically filter file types.

### Q: Parsing results incorrect?
A: Please check if the text format meets requirements, ensure ID lines start with `M` followed by number (e.g., `M24_`, `M35_`, `M100_`).

### Q: CSV export failed?
A: Make sure you have sufficient disk space and file write permissions.

## Automated Build and Release

The project uses GitHub Actions for automated building and releasing. When you create and push a version tag, the build process will be triggered automatically.

### Creating Release Version

**Method 1: Trigger via Tag (Recommended)**

```bash
# Create version tag
git tag v1.1.0

# Push tag to GitHub
git push origin v1.1.0
```

After pushing the tag, GitHub Actions will automatically:
1. Build the application
2. Package as single-file executable
3. Create GitHub Release
4. Upload release package

**Method 2: Manual Trigger**

1. Visit the GitHub repository's Actions page
2. Select "Build and Release" workflow
3. Click "Run workflow"
4. Enter version number (e.g., 1.1.0)
5. Click "Run workflow" button

### Download Release Version

Visit [GitHub Releases](https://github.com/JiashuaiXu/TagAnalyzer-/releases) to download the latest pre-built executable.

## Changelog

### v1.1.0 (2024-11-04)
- 🔧 **Format Support Extension**: Supports all M-prefixed number formats (M24, M35, M1, M100, etc.)
- ✨ Unified processing of M-prefixed number formats, no longer limited to M35
- 📝 Updated parsing rules to support more flexible file formats
- 📋 **Version Control**: Added version information display and About window
- 🚀 **CI/CD**: Added GitHub Actions automated build and release workflow

### v1.0.0 (2024-01-XX)
- ✨ Initial release
- 🎯 Tag parsing and statistics support
- 🖥️ Modern GUI interface
- 📄 CSV export functionality
- 🌐 Cross-platform support
- 🚀 Using .NET 10 + C# 14 latest features

---

**Contact**: jiashuai_xu@qq.com
