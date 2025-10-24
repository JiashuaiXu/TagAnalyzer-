# MVVM 模式实现详解 (.NET 10 + C# 14)

## 📋 目录

1. [MVVM 模式概述](#mvvm-模式概述)
2. [Model 层实现](#model-层实现)
3. [ViewModel 层实现](#viewmodel-层实现)
4. [View 层实现](#view-层实现)
5. [数据绑定详解](#数据绑定详解)
6. [命令模式实现](#命令模式实现)
7. [依赖注入应用](#依赖注入应用)
8. [测试策略](#测试策略)
9. [批量处理功能的MVVM实现](#批量处理功能的mvvm实现)

---

## 🏗️ MVVM 模式概述

### 什么是 MVVM？

MVVM (Model-View-ViewModel) 是一种架构模式，将应用程序分为三个主要层次：

```
┌─────────────┐    ┌──────────────┐    ┌─────────────┐
│    View     │◄──►│  ViewModel   │◄──►│    Model    │
│   (视图)     │    │  (视图模型)   │    │   (模型)    │
│             │    │              │    │             │
│ • UI 控件    │    │ • 业务逻辑    │    │ • 数据模型   │
│ • 用户交互   │    │ • 状态管理    │    │ • 数据访问   │
│ • 数据展示   │    │ • 命令处理    │    │ • 业务规则   │
└─────────────┘    └──────────────┘    └─────────────┘
```

### MVVM 的优势

1. **关注点分离**：UI、业务逻辑、数据访问分离
2. **可测试性**：ViewModel 可以独立测试
3. **可维护性**：代码结构清晰，易于维护
4. **可重用性**：ViewModel 可以在不同 View 中重用
5. **数据绑定**：自动同步 UI 和数据状态

### MVVM 在 Avalonia 中的应用

```csharp
// 项目结构
TagAnalyzer/
├── Models/                    # Model 层
│   ├── TagInfo.cs            # 数据模型
│   └── TextParser.cs         # 业务逻辑
├── ViewModels/               # ViewModel 层
│   ├── MainWindowViewModel.cs # 主视图模型
│   └── TagInfoViewModel.cs   # 数据视图模型
├── Views/                    # View 层
│   ├── MainWindow.axaml      # 主窗口界面
│   └── MainWindow.axaml.cs   # 主窗口代码
└── Services/                 # 服务层
    └── FileService.cs        # 文件服务
```

---

## 📊 Model 层实现

### 1. 数据模型 (Data Models)

#### 基础数据模型 (C# 14 增强)
```csharp
// TagInfo.cs - 标签信息模型 (使用 C# 14 特性)
namespace TagAnalyzer.Models;

public class TagInfo
{
    // C# 14 必需成员
    public required string Tag { get; set; }
    public required int Count { get; set; }
    
    // C# 14 集合表达式初始化
    public List<string> SourceIds { get; set; } = [];
    
    // 必需构造函数
    public TagInfo(string tag, int count)
    {
        Tag = tag;
        Count = count;
    }
    
    // C# 14 模式匹配重写
    public override string ToString() => $"{Tag}: {Count} 次";
    
    public override bool Equals(object? obj) => obj is TagInfo other && Tag == other.Tag;
    
    public override int GetHashCode() => Tag.GetHashCode();
    
    // C# 14 解构方法
    public void Deconstruct(out string tag, out int count, out List<string> sourceIds)
    {
        tag = Tag;
        count = Count;
        sourceIds = SourceIds;
    }
}
```

#### 复杂数据模型
```csharp
// FileInfo.cs - 文件信息模型
using System;
using System.IO;

namespace TagAnalyzer.Models;

public class FileInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
    public FileStatus Status { get; set; } = FileStatus.NotLoaded;
    
    public string FileSizeFormatted => FormatFileSize(FileSize);
    
    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

public enum FileStatus
{
    NotLoaded,
    Loading,
    Loaded,
    Error
}
```

### 2. 业务逻辑 (Business Logic)

#### 文本解析器
```csharp
// TextParser.cs - 文本解析业务逻辑
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TagAnalyzer.Models;

public class TextParser
{
    // 正则表达式模式
    private static readonly Regex IdPattern = new(@"M35_\d{6}", RegexOptions.Compiled);
    private static readonly Regex TagPattern = new(@"【([^】]+)】", RegexOptions.Compiled);
    
    // 解析结果
    public class ParseResult
    {
        public List<TagInfo> TagInfos { get; set; } = new();
        public int TotalLines { get; set; }
        public int ProcessedLines { get; set; }
        public List<string> Errors { get; set; } = new();
        public TimeSpan ProcessingTime { get; set; }
    }
    
    // 主要解析方法
    public static ParseResult ParseText(string text)
    {
        var startTime = DateTime.Now;
        var result = new ParseResult();
        var tagDictionary = new Dictionary<string, TagInfo>();
        
        try
        {
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            result.TotalLines = lines.Length;
            
            foreach (var line in lines)
            {
                var processedLine = ProcessLine(line, tagDictionary);
                if (processedLine)
                {
                    result.ProcessedLines++;
                }
            }
            
            result.TagInfos = tagDictionary.Values
                .OrderBy(t => t.Tag)
                .ToList();
        }
        catch (Exception ex)
        {
            result.Errors.Add($"解析错误: {ex.Message}");
        }
        finally
        {
            result.ProcessingTime = DateTime.Now - startTime;
        }
        
        return result;
    }
    
    // 处理单行
    private static bool ProcessLine(string line, Dictionary<string, TagInfo> tagDictionary)
    {
        var trimmedLine = line.Trim();
        
        // 跳过拼音行（以制表符开头）
        if (trimmedLine.StartsWith('\t'))
            return false;
        
        // 查找ID
        var idMatch = IdPattern.Match(trimmedLine);
        if (!idMatch.Success)
            return false;
        
        var id = idMatch.Value;
        
        // 查找所有标签
        var tagMatches = TagPattern.Matches(trimmedLine);
        foreach (Match tagMatch in tagMatches)
        {
            var tag = tagMatch.Groups[1].Value;
            
            if (!tagDictionary.ContainsKey(tag))
            {
                tagDictionary[tag] = new TagInfo
                {
                    Tag = tag,
                    Count = 0,
                    SourceIds = new List<string>()
                };
            }
            
            tagDictionary[tag].Count++;
            if (!tagDictionary[tag].SourceIds.Contains(id))
            {
                tagDictionary[tag].SourceIds.Add(id);
            }
        }
        
        return tagMatches.Count > 0;
    }
}
```

### 3. 数据访问 (Data Access)

#### 文件服务
```csharp
// FileService.cs - 文件访问服务
using System;
using System.IO;
using System.Threading.Tasks;

namespace TagAnalyzer.Services;

public interface IFileService
{
    Task<string> ReadTextAsync(string filePath);
    Task WriteTextAsync(string filePath, string content);
    Task<bool> FileExistsAsync(string filePath);
    Task<FileInfo> GetFileInfoAsync(string filePath);
}

public class FileService : IFileService
{
    public async Task<string> ReadTextAsync(string filePath)
    {
        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new FileServiceException($"读取文件失败: {ex.Message}", ex);
        }
    }
    
    public async Task WriteTextAsync(string filePath, string content)
    {
        try
        {
            await File.WriteAllTextAsync(filePath, content);
        }
        catch (Exception ex)
        {
            throw new FileServiceException($"写入文件失败: {ex.Message}", ex);
        }
    }
    
    public async Task<bool> FileExistsAsync(string filePath)
    {
        return await Task.FromResult(File.Exists(filePath));
    }
    
    public async Task<FileInfo> GetFileInfoAsync(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        return await Task.FromResult(fileInfo);
    }
}

public class FileServiceException : Exception
{
    public FileServiceException(string message) : base(message) { }
    public FileServiceException(string message, Exception innerException) : base(message, innerException) { }
}
```

---

## 🎛️ ViewModel 层实现

### 1. 基础 ViewModel

#### INotifyPropertyChanged 实现
```csharp
// BaseViewModel.cs - 基础视图模型
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TagAnalyzer.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
            return false;
            
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    // 批量属性更新
    protected virtual void OnPropertiesChanged(params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            OnPropertyChanged(propertyName);
        }
    }
}
```

#### 主窗口 ViewModel
```csharp
// MainWindowViewModel.cs - 主窗口视图模型
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TagAnalyzer.Models;
using TagAnalyzer.Services;

namespace TagAnalyzer.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly IFileService _fileService;
    
    // 私有字段
    private string _statusMessage = "请选择文本文件进行分析";
    private bool _isProcessing = false;
    private string _selectedFilePath = string.Empty;
    private ObservableCollection<TagInfoViewModel> _tagInfos = new();
    
    // 公共属性
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    
    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }
    
    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set => SetProperty(ref _selectedFilePath, value);
    }
    
    public ObservableCollection<TagInfoViewModel> TagInfos
    {
        get => _tagInfos;
        set => SetProperty(ref _tagInfos, value);
    }
    
    // 计算属性
    public bool HasData => TagInfos.Count > 0;
    public int TotalTags => TagInfos.Count;
    public int TotalOccurrences => TagInfos.Sum(t => t.Count);
    
    // 构造函数
    public MainWindowViewModel(IFileService fileService)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }
    
    // 业务方法
    public async Task ProcessFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;
            
        IsProcessing = true;
        StatusMessage = "正在分析文件...";
        SelectedFilePath = filePath;
        
        try
        {
            var text = await _fileService.ReadTextAsync(filePath);
            var result = TextParser.ParseText(text);
            
            TagInfos.Clear();
            foreach (var tagInfo in result.TagInfos)
            {
                TagInfos.Add(new TagInfoViewModel(tagInfo));
            }
            
            StatusMessage = $"分析完成！找到 {result.TagInfos.Count} 个标签，共 {result.ProcessedLines} 行数据";
            OnPropertiesChanged(nameof(HasData), nameof(TotalTags), nameof(TotalOccurrences));
        }
        catch (Exception ex)
        {
            StatusMessage = $"分析失败：{ex.Message}";
            TagInfos.Clear();
        }
        finally
        {
            IsProcessing = false;
        }
    }
    
    public void ClearResults()
    {
        TagInfos.Clear();
        SelectedFilePath = string.Empty;
        StatusMessage = "请选择文本文件进行分析";
        OnPropertiesChanged(nameof(HasData), nameof(TotalTags), nameof(TotalOccurrences));
    }
}
```

### 2. 数据 ViewModel

#### 标签信息 ViewModel
```csharp
// TagInfoViewModel.cs - 标签信息视图模型
using System;
using System.Linq;
using TagAnalyzer.Models;

namespace TagAnalyzer.ViewModels;

public class TagInfoViewModel : BaseViewModel
{
    private readonly TagInfo _model;
    
    // 公共属性
    public string Tag
    {
        get => _model.Tag;
        set
        {
            if (_model.Tag != value)
            {
                _model.Tag = value;
                OnPropertyChanged();
            }
        }
    }
    
    public int Count
    {
        get => _model.Count;
        set
        {
            if (_model.Count != value)
            {
                _model.Count = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string SourceIds
    {
        get => string.Join(", ", _model.SourceIds);
    }
    
    public string SourceIdsCount => $"{_model.SourceIds.Count} 个来源";
    
    // 构造函数
    public TagInfoViewModel(TagInfo model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }
    
    // 更新源ID列表
    public void UpdateSourceIds()
    {
        OnPropertyChanged(nameof(SourceIds));
        OnPropertyChanged(nameof(SourceIdsCount));
    }
}
```

### 3. 命令实现

#### 基础命令
```csharp
// RelayCommand.cs - 中继命令
using System;
using System.Windows.Input;

namespace TagAnalyzer.Commands;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }
    
    public void Execute(object? parameter)
    {
        _execute();
    }
    
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;
    
    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke((T?)parameter) ?? true;
    }
    
    public void Execute(object? parameter)
    {
        _execute((T?)parameter);
    }
    
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

#### 异步命令
```csharp
// AsyncRelayCommand.cs - 异步中继命令
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TagAnalyzer.Commands;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;
    
    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }
    
    public async void Execute(object? parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }
    }
    
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

---

## 🖼️ View 层实现

### 1. XAML 界面设计

#### 主窗口界面
```xml
<!-- MainWindow.axaml -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:vm="using:TagAnalyzer.ViewModels"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="600"
        x:Class="TagAnalyzer.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Title="标签分析工具 - jiashuai_xu@qq.com"
        Width="900" Height="700"
        MinWidth="600" MinHeight="400">

    <Design.DataContext>
        <vm:MainWindowViewModel/>
    </Design.DataContext>

    <Grid RowDefinitions="Auto,*,Auto">
        <!-- 工具栏 -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10" Spacing="10">
            <Button Name="SelectFileButton" 
                    Content="选择文件" 
                    Click="SelectFileButton_Click"
                    IsEnabled="{Binding !IsProcessing}"
                    Classes="accent"/>
            <Button Name="ExportCsvButton" 
                    Content="导出CSV" 
                    Click="ExportCsvButton_Click"
                    IsEnabled="{Binding HasData}"
                    Classes="outline"/>
            <Button Name="ClearButton" 
                    Content="清空结果" 
                    Click="ClearButton_Click"
                    IsEnabled="{Binding HasData}"
                    Classes="outline"/>
        </StackPanel>

        <!-- 主内容区域 -->
        <Grid Grid.Row="1" Margin="10">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>

            <!-- 状态信息 -->
            <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,10">
                <TextBlock Text="{Binding StatusMessage}" 
                           FontSize="14" 
                           Foreground="{DynamicResource SystemAccentColor}"/>
                <TextBlock Text="{Binding TotalTags, StringFormat=' | 标签: {0}'}" 
                           FontSize="14" 
                           Margin="10,0,0,0"/>
                <TextBlock Text="{Binding TotalOccurrences, StringFormat=' | 总计: {0}'}" 
                           FontSize="14" 
                           Margin="10,0,0,0"/>
            </StackPanel>

            <!-- 数据列表 -->
            <ListBox Grid.Row="1" 
                     ItemsSource="{Binding TagInfos}"
                     ScrollViewer.HorizontalScrollBarVisibility="Auto"
                     ScrollViewer.VerticalScrollBarVisibility="Auto">
                <ListBox.ItemTemplate>
                    <DataTemplate>
                        <Grid Margin="5">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="150"/>
                                <ColumnDefinition Width="100"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <TextBlock Grid.Column="0" 
                                       Text="{Binding Tag}" 
                                       FontWeight="Bold"/>
                            <TextBlock Grid.Column="1" 
                                       Text="{Binding Count}" 
                                       HorizontalAlignment="Center"/>
                            <TextBlock Grid.Column="2" 
                                       Text="{Binding SourceIds}" 
                                       TextWrapping="Wrap"/>
                        </Grid>
                    </DataTemplate>
                </ListBox.ItemTemplate>
            </ListBox>
        </Grid>

        <!-- 底部状态栏 -->
        <Border Grid.Row="2" Background="{DynamicResource SystemControlBackgroundBaseLowBrush}" 
                Padding="10,5">
            <StackPanel Orientation="Horizontal" Spacing="10">
                <TextBlock Text="作者：jiashuai_xu@qq.com" 
                           FontSize="12" 
                           Foreground="{DynamicResource SystemBaseMediumColor}"/>
                <TextBlock Text="|" 
                           FontSize="12" 
                           Foreground="{DynamicResource SystemBaseMediumColor}"/>
                <TextBlock Text="版本：1.0.0" 
                           FontSize="12" 
                           Foreground="{DynamicResource SystemBaseMediumColor}"/>
            </StackPanel>
        </Border>
    </Grid>
</Window>
```

### 2. 代码后台

#### 主窗口代码后台
```csharp
// MainWindow.axaml.cs
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CsvHelper;
using TagAnalyzer.ViewModels;

namespace TagAnalyzer;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel(new Services.FileService());
        DataContext = _viewModel;
    }

    private async void SelectFileButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var options = new FilePickerOpenOptions
            {
                Title = "选择文本文件",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("文本文件")
                    {
                        Patterns = new[] { "*.txt" }
                    },
                    new FilePickerFileType("所有文件")
                    {
                        Patterns = new[] { "*.*" }
                    }
                }
            };

            var files = await StorageProvider.OpenFilePickerAsync(options);
            
            if (files.Count > 0)
            {
                var file = files[0];
                await _viewModel.ProcessFileAsync(file.Path.LocalPath);
            }
        }
        catch (Exception ex)
        {
            _viewModel.StatusMessage = $"文件选择失败：{ex.Message}";
        }
    }

    private async void ExportCsvButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var options = new FilePickerSaveOptions
            {
                Title = "保存CSV文件",
                DefaultExtension = "csv",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("CSV文件")
                    {
                        Patterns = new[] { "*.csv" }
                    }
                }
            };

            var file = await StorageProvider.SaveFilePickerAsync(options);
            
            if (file != null)
            {
                await using var writer = new StreamWriter(file.Path.LocalPath);
                await using var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
                
                // 写入标题
                csv.WriteField("标签");
                csv.WriteField("出现次数");
                csv.WriteField("来源ID");
                await csv.NextRecordAsync();

                // 写入数据
                foreach (var tagInfo in _viewModel.TagInfos)
                {
                    csv.WriteField(tagInfo.Tag);
                    csv.WriteField(tagInfo.Count);
                    csv.WriteField(tagInfo.SourceIds);
                    await csv.NextRecordAsync();
                }

                _viewModel.StatusMessage = $"CSV文件已保存到：{file.Path.LocalPath}";
            }
        }
        catch (Exception ex)
        {
            _viewModel.StatusMessage = $"CSV导出失败：{ex.Message}";
        }
    }

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.ClearResults();
    }
}
```

---

## 🔗 数据绑定详解

### 1. 绑定模式

#### 单向绑定
```xml
<!-- 从 ViewModel 到 View -->
<TextBlock Text="{Binding StatusMessage}"/>
<TextBlock Text="{Binding StatusMessage, Mode=OneWay}"/>
```

#### 双向绑定
```xml
<!-- ViewModel 和 View 相互同步 -->
<TextBox Text="{Binding UserInput, Mode=TwoWay}"/>
```

#### 一次性绑定
```xml
<!-- 只在初始化时绑定一次 -->
<TextBlock Text="{Binding StaticValue, Mode=OneTime}"/>
```

### 2. 绑定路径

#### 简单属性绑定
```xml
<TextBlock Text="{Binding Name}"/>
<TextBlock Text="{Binding Age}"/>
```

#### 嵌套属性绑定
```xml
<TextBlock Text="{Binding User.Profile.Name}"/>
<TextBlock Text="{Binding Settings.Theme.Color}"/>
```

#### 集合索引绑定
```xml
<TextBlock Text="{Binding Items[0].Name}"/>
<TextBlock Text="{Binding Users[SelectedIndex].Email}"/>
```

### 3. 绑定转换器

#### 创建转换器
```csharp
// BoolToVisibilityConverter.cs
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia;

namespace TagAnalyzer.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Hidden;
        }
        return Visibility.Hidden;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}
```

#### 使用转换器
```xml
<!-- 在资源中定义 -->
<Window.Resources>
    <converters:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
</Window.Resources>

<!-- 使用转换器 -->
<TextBlock Visibility="{Binding IsVisible, Converter={StaticResource BoolToVisibilityConverter}}"/>
```

### 4. 绑定验证

#### 实现验证规则
```csharp
// NotEmptyValidationRule.cs
using System;
using System.Globalization;
using Avalonia.Data;

namespace TagAnalyzer.Validation;

public class NotEmptyValidationRule : IValidationRule
{
    public string? ErrorMessage { get; set; } = "字段不能为空";

    public ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
        {
            return ValidationResult.ValidResult;
        }
        
        return new ValidationResult(false, ErrorMessage);
    }
}
```

#### 使用验证规则
```xml
<TextBox>
    <TextBox.Text>
        <Binding Path="UserName" Mode="TwoWay">
            <Binding.ValidationRules>
                <validation:NotEmptyValidationRule ErrorMessage="用户名不能为空"/>
            </Binding.ValidationRules>
        </Binding>
    </TextBox.Text>
</TextBox>
```

---

## ⚡ 命令模式实现

### 1. 命令绑定

#### ViewModel 中的命令
```csharp
// MainWindowViewModel.cs
public class MainWindowViewModel : BaseViewModel
{
    private ICommand? _selectFileCommand;
    private ICommand? _exportCsvCommand;
    private ICommand? _clearCommand;
    
    public ICommand SelectFileCommand => _selectFileCommand ??= new AsyncRelayCommand(
        async () => await SelectFileAsync(),
        () => !IsProcessing);
    
    public ICommand ExportCsvCommand => _exportCsvCommand ??= new RelayCommand(
        () => ExportCsv(),
        () => HasData);
    
    public ICommand ClearCommand => _clearCommand ??= new RelayCommand(
        () => ClearResults(),
        () => HasData);
    
    // 命令实现方法
    private async Task SelectFileAsync()
    {
        // 文件选择逻辑
    }
    
    private void ExportCsv()
    {
        // CSV导出逻辑
    }
    
    private void ClearResults()
    {
        // 清空结果逻辑
    }
}
```

#### XAML 中的命令绑定
```xml
<!-- 使用命令绑定 -->
<Button Command="{Binding SelectFileCommand}" Content="选择文件"/>
<Button Command="{Binding ExportCsvCommand}" Content="导出CSV"/>
<Button Command="{Binding ClearCommand}" Content="清空结果"/>
```

### 2. 参数化命令

#### 带参数的命令
```csharp
// 参数化命令
public ICommand DeleteItemCommand => new RelayCommand<TagInfoViewModel>(
    item => DeleteItem(item),
    item => item != null);

private void DeleteItem(TagInfoViewModel item)
{
    if (item != null)
    {
        TagInfos.Remove(item);
    }
}
```

#### 使用参数化命令
```xml
<Button Command="{Binding DeleteItemCommand}" 
        CommandParameter="{Binding}"
        Content="删除"/>
```

---

## 💉 依赖注入应用

### 1. 服务注册

#### 应用程序启动时注册服务
```csharp
// App.axaml.cs
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        // 配置服务
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }
    
    private void ConfigureServices(IServiceCollection services)
    {
        // 注册服务
        services.AddSingleton<IFileService, FileService>();
        services.AddTransient<MainWindowViewModel>();
        
        // 注册窗口
        services.AddTransient<MainWindow>();
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _serviceProvider?.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
        }
        base.OnFrameworkInitializationCompleted();
    }
    
    public override void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(sender, e);
    }
}
```

### 2. 构造函数注入

#### ViewModel 构造函数注入
```csharp
// MainWindowViewModel.cs
public class MainWindowViewModel : BaseViewModel
{
    private readonly IFileService _fileService;
    private readonly ILogger<MainWindowViewModel> _logger;
    
    public MainWindowViewModel(
        IFileService fileService,
        ILogger<MainWindowViewModel> logger)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

#### View 构造函数注入
```csharp
// MainWindow.axaml.cs
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        DataContext = _viewModel;
    }
}
```

---

## 🧪 测试策略

### 1. ViewModel 单元测试

#### 测试项目结构
```
TagAnalyzer.Tests/
├── ViewModels/
│   └── MainWindowViewModelTests.cs
├── Models/
│   └── TextParserTests.cs
├── Services/
│   └── FileServiceTests.cs
└── TagAnalyzer.Tests.csproj
```

#### ViewModel 测试示例
```csharp
// MainWindowViewModelTests.cs
using Microsoft.Extensions.Logging;
using Moq;
using TagAnalyzer.Models;
using TagAnalyzer.Services;
using TagAnalyzer.ViewModels;
using Xunit;

namespace TagAnalyzer.Tests.ViewModels;

public class MainWindowViewModelTests
{
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<ILogger<MainWindowViewModel>> _mockLogger;
    private readonly MainWindowViewModel _viewModel;
    
    public MainWindowViewModelTests()
    {
        _mockFileService = new Mock<IFileService>();
        _mockLogger = new Mock<ILogger<MainWindowViewModel>>();
        _viewModel = new MainWindowViewModel(_mockFileService.Object, _mockLogger.Object);
    }
    
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Assert
        Assert.Equal("请选择文本文件进行分析", _viewModel.StatusMessage);
        Assert.False(_viewModel.IsProcessing);
        Assert.Empty(_viewModel.TagInfos);
    }
    
    [Fact]
    public async Task ProcessFileAsync_WithValidFile_ShouldProcessSuccessfully()
    {
        // Arrange
        var filePath = "test.txt";
        var testText = "M35_230001【抽泣】测试内容";
        _mockFileService.Setup(x => x.ReadTextAsync(filePath))
                       .ReturnsAsync(testText);
        
        // Act
        await _viewModel.ProcessFileAsync(filePath);
        
        // Assert
        Assert.Single(_viewModel.TagInfos);
        Assert.Equal("抽泣", _viewModel.TagInfos[0].Tag);
        Assert.Equal(1, _viewModel.TagInfos[0].Count);
    }
    
    [Fact]
    public async Task ProcessFileAsync_WithInvalidFile_ShouldHandleError()
    {
        // Arrange
        var filePath = "invalid.txt";
        _mockFileService.Setup(x => x.ReadTextAsync(filePath))
                       .ThrowsAsync(new FileNotFoundException());
        
        // Act
        await _viewModel.ProcessFileAsync(filePath);
        
        // Assert
        Assert.Empty(_viewModel.TagInfos);
        Assert.Contains("分析失败", _viewModel.StatusMessage);
    }
    
    [Fact]
    public void ClearResults_ShouldResetViewModel()
    {
        // Arrange
        _viewModel.TagInfos.Add(new TagInfoViewModel(new TagInfo("测试", 1, new List<string>())));
        
        // Act
        _viewModel.ClearResults();
        
        // Assert
        Assert.Empty(_viewModel.TagInfos);
        Assert.Equal("请选择文本文件进行分析", _viewModel.StatusMessage);
    }
}
```

### 2. Model 测试

#### 文本解析器测试
```csharp
// TextParserTests.cs
using TagAnalyzer.Models;
using Xunit;

namespace TagAnalyzer.Tests.Models;

public class TextParserTests
{
    [Fact]
    public void ParseText_WithValidInput_ShouldReturnCorrectResults()
    {
        // Arrange
        var text = @"M35_230001【抽泣】测试内容
	拼音行，应该被忽略
M35_230002【叹气】【抽泣】另一行内容";
        
        // Act
        var result = TextParser.ParseText(text);
        
        // Assert
        Assert.Equal(2, result.TagInfos.Count);
        Assert.Equal(2, result.ProcessedLines);
        
        var tagInfo = result.TagInfos.FirstOrDefault(t => t.Tag == "抽泣");
        Assert.NotNull(tagInfo);
        Assert.Equal(2, tagInfo.Count);
        Assert.Equal(2, tagInfo.SourceIds.Count);
    }
    
    [Fact]
    public void ParseText_WithEmptyInput_ShouldReturnEmptyResults()
    {
        // Arrange
        var text = "";
        
        // Act
        var result = TextParser.ParseText(text);
        
        // Assert
        Assert.Empty(result.TagInfos);
        Assert.Equal(0, result.ProcessedLines);
    }
    
    [Fact]
    public void ParseText_WithNoTags_ShouldReturnEmptyResults()
    {
        // Arrange
        var text = @"M35_230001没有标签的内容
	拼音行
M35_230002另一行没有标签";
        
        // Act
        var result = TextParser.ParseText(text);
        
        // Assert
        Assert.Empty(result.TagInfos);
        Assert.Equal(0, result.ProcessedLines);
    }
}
```

### 3. 集成测试

#### 集成测试示例
```csharp
// IntegrationTests.cs
using Microsoft.Extensions.DependencyInjection;
using TagAnalyzer.Services;
using TagAnalyzer.ViewModels;
using Xunit;

namespace TagAnalyzer.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task FullWorkflow_ShouldWorkCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IFileService, FileService>();
        services.AddTransient<MainWindowViewModel>();
        
        var serviceProvider = services.BuildServiceProvider();
        var viewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
        
        // Act & Assert
        // 这里可以测试完整的用户工作流程
        Assert.NotNull(viewModel);
    }
}
```

---

## 🎯 总结

MVVM 模式在 Avalonia 中的应用包括：

1. **Model 层**：数据模型、业务逻辑、数据访问
2. **ViewModel 层**：视图模型、命令、状态管理
3. **View 层**：XAML 界面、代码后台、数据绑定
4. **数据绑定**：单向、双向、转换器、验证
5. **命令模式**：ICommand 实现、参数化命令
6. **依赖注入**：服务注册、构造函数注入
7. **测试策略**：单元测试、集成测试

1. **Model 层**：数据模型、业务逻辑、数据访问
2. **ViewModel 层**：视图模型、命令、状态管理
3. **View 层**：XAML 界面、代码后台、数据绑定
4. **数据绑定**：单向、双向、转换器、验证
5. **命令模式**：ICommand 实现、参数化命令
6. **依赖注入**：服务注册、构造函数注入
7. **测试策略**：单元测试、集成测试
8. **批量处理**：异步处理、进度反馈、数据合并

通过 MVVM 模式，可以创建出结构清晰、易于测试、可维护的 Avalonia 应用程序！

---

## 🚀 批量处理功能的MVVM实现

### 1. 批量处理架构设计

#### MVVM 三层架构在批量处理中的应用

```
┌─────────────────────────────────────────────────────────────┐
│                    批量处理 MVVM 架构                        │
├─────────────────────────────────────────────────────────────┤
│  View Layer (视图层)                                       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   MainWindow    │  │   ProgressBar   │  │   StatusUI   │ │
│  │   (主界面)       │  │   (进度显示)    │  │   (状态信息)  │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
├─────────────────────────────────────────────────────────────┤
│  ViewModel Layer (视图模型层)                              │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │ MainWindowVM    │  │ BatchProcessor  │  │ DataMerger   │ │
│  │ (主视图模型)     │  │ (批量处理器)    │  │ (数据合并器)  │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
├─────────────────────────────────────────────────────────────┤
│  Model Layer (模型层)                                      │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   FileScanner   │  │   TextParser    │  │   TagInfo    │ │
│  │   (文件扫描器)   │  │   (文本解析器)   │  │   (标签信息)  │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### 2. ViewModel 层实现

#### 批量处理专用属性

```csharp
// ViewModels/MainWindowViewModel.cs - 批量处理扩展
public class MainWindowViewModel : INotifyPropertyChanged
{
    // 基础属性
    private ObservableCollection<TagInfoViewModel> _tagInfos = new();
    private string _statusMessage = "请选择文件或文件夹进行分析";
    private bool _isProcessing = false;
    
    // 批量处理专用属性
    private int _processedFiles = 0;
    private int _totalFiles = 0;
    private string _currentProcessingFile = string.Empty;
    private DateTime _startTime = DateTime.Now;
    
    // 属性实现 - 使用 C# 14 特性
    public ObservableCollection<TagInfoViewModel> TagInfos
    {
        get => _tagInfos;
        set
        {
            _tagInfos = value;
            OnPropertyChanged();
        }
    }
    
    public int ProcessedFiles
    {
        get => _processedFiles;
        set
        {
            _processedFiles = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ProgressMessage)); // 触发计算属性更新
            OnPropertyChanged(nameof(ProcessingSpeed)); // 触发处理速度更新
        }
    }
    
    public int TotalFiles
    {
        get => _totalFiles;
        set
        {
            _totalFiles = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ProgressMessage));
        }
    }
    
    public string CurrentProcessingFile
    {
        get => _currentProcessingFile;
        set
        {
            _currentProcessingFile = value;
            OnPropertyChanged();
        }
    }
    
    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            OnPropertyChanged();
        }
    }
    
    // 计算属性 - C# 14 特性
    public string ProgressMessage
    {
        get
        {
            if (_totalFiles > 0)
            {
                var percentage = _processedFiles * 100.0 / _totalFiles;
                return $"进度: {_processedFiles}/{_totalFiles} ({percentage:F1}%)";
            }
            return string.Empty;
        }
    }
    
    public string ProcessingSpeed
    {
        get
        {
            if (_processedFiles > 0 && _isProcessing)
            {
                var elapsed = DateTime.Now - _startTime;
                var speed = _processedFiles / elapsed.TotalSeconds;
                return $"处理速度: {speed:F1} 文件/秒";
            }
            return string.Empty;
        }
    }
    
    public TimeSpan EstimatedTimeRemaining
    {
        get
        {
            if (_processedFiles > 0 && _totalFiles > _processedFiles && _isProcessing)
            {
                var elapsed = DateTime.Now - _startTime;
                var speed = _processedFiles / elapsed.TotalSeconds;
                var remaining = _totalFiles - _processedFiles;
                return TimeSpan.FromSeconds(remaining / speed);
            }
            return TimeSpan.Zero;
        }
    }
}
```

#### 批量处理核心方法

```csharp
// 批量处理核心方法 - 使用 C# 14 异步模式
public async Task ProcessFolderAsync(string folderPath)
{
    // 初始化处理状态
    IsProcessing = true;
    StartTime = DateTime.Now;
    StatusMessage = "正在扫描文件夹...";
    ProcessedFiles = 0;
    TotalFiles = 0;
    CurrentProcessingFile = string.Empty;

    try
    {
        // 1. 文件扫描阶段
        var txtFiles = await ScanTxtFilesAsync(folderPath);
        TotalFiles = txtFiles.Length;

        if (TotalFiles == 0)
        {
            StatusMessage = "文件夹中没有找到txt文件";
            return;
        }

        StatusMessage = $"找到 {TotalFiles} 个txt文件，开始批量处理...";

        // 2. 数据合并容器 - 使用 C# 14 集合表达式
        var allTagInfos = new Dictionary<string, TagInfoViewModel>();

        // 3. 逐个文件处理
        foreach (var filePath in txtFiles)
        {
            try
            {
                // 更新当前处理文件
                CurrentProcessingFile = Path.GetFileName(filePath);
                StatusMessage = $"正在处理: {CurrentProcessingFile}";

                // 异步读取文件内容
                var content = await File.ReadAllTextAsync(filePath);
                var results = TextParser.ParseText(content);

                // 合并结果到总容器
                MergeResults(allTagInfos, results, filePath);
            }
            catch (Exception ex)
            {
                StatusMessage = $"处理文件 {Path.GetFileName(filePath)} 失败: {ex.Message}";
            }

            // 更新进度
            ProcessedFiles++;
        }

        // 4. 更新UI显示
        UpdateUIWithResults(allTagInfos);
        StatusMessage = $"批量处理完成！共处理 {ProcessedFiles} 个文件，找到 {allTagInfos.Count} 个不同的标签";
    }
    catch (Exception ex)
    {
        StatusMessage = $"批量处理失败：{ex.Message}";
    }
    finally
    {
        IsProcessing = false;
        CurrentProcessingFile = string.Empty;
    }
}

// 文件扫描方法
private async Task<string[]> ScanTxtFilesAsync(string folderPath)
{
    return await Task.Run(() => 
        Directory.GetFiles(folderPath, "*.txt", SearchOption.AllDirectories));
}

// 数据合并方法 - 使用 C# 14 模式匹配
private void MergeResults(Dictionary<string, TagInfoViewModel> allTagInfos, 
                         List<TagInfo> results, string filePath)
{
    foreach (var result in results)
    {
        // 使用模式匹配处理不同情况
        var tagInfo = result switch
        {
            { Tag: var tag, Count: > 0 } when !string.IsNullOrEmpty(tag) =>
                GetOrCreateTagInfo(allTagInfos, tag),
            _ => null
        };
        
        if (tagInfo != null)
        {
            tagInfo.Count += result.Count;
            MergeSourceIds(tagInfo, result.SourceIds);
            MergeSourceFiles(tagInfo, filePath);
        }
    }
}

// 获取或创建标签信息
private TagInfoViewModel GetOrCreateTagInfo(Dictionary<string, TagInfoViewModel> allTagInfos, string tag)
{
    if (!allTagInfos.ContainsKey(tag))
    {
        allTagInfos[tag] = new TagInfoViewModel
        {
            Tag = tag,
            Count = 0,
            SourceIds = string.Empty,
            SourceFiles = string.Empty
        };
    }
    return allTagInfos[tag];
}

// 合并来源ID
private void MergeSourceIds(TagInfoViewModel tagInfo, List<string> newIds)
{
    if (string.IsNullOrEmpty(tagInfo.SourceIds))
    {
        tagInfo.SourceIds = string.Join(", ", newIds);
    }
    else
    {
        var existingIds = tagInfo.SourceIds.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList();
        existingIds.AddRange(newIds);
        tagInfo.SourceIds = string.Join(", ", existingIds.Distinct());
    }
}

// 合并来源文件
private void MergeSourceFiles(TagInfoViewModel tagInfo, string filePath)
{
    var fileName = Path.GetFileName(filePath);
    if (string.IsNullOrEmpty(tagInfo.SourceFiles))
    {
        tagInfo.SourceFiles = fileName;
    }
    else if (!tagInfo.SourceFiles.Contains(fileName))
    {
        tagInfo.SourceFiles += $", {fileName}";
    }
}

// UI更新方法
private void UpdateUIWithResults(Dictionary<string, TagInfoViewModel> allTagInfos)
{
    TagInfos.Clear();
    foreach (var tagInfo in allTagInfos.Values.OrderBy(t => t.Tag))
    {
        TagInfos.Add(tagInfo);
    }
}
```

### 3. View 层实现

#### XAML 数据绑定

```xml
<!-- MainWindow.axaml - 批量处理UI -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:TagAnalyzer.ViewModels"
        x:Class="TagAnalyzer.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Title="标签分析工具 - 批量处理版 - jiashuai_xu@qq.com"
        Width="1000" Height="800"
        MinWidth="800" MinHeight="600">

    <Grid RowDefinitions="Auto,*,Auto">
        <!-- 工具栏 -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10" Spacing="10">
            <Button Name="SelectFileButton" Content="选择文件" 
                    Click="SelectFileButton_Click"
                    IsEnabled="{Binding !IsProcessing}"
                    Classes="accent"/>
            <Button Name="SelectFolderButton" Content="选择文件夹" 
                    Click="SelectFolderButton_Click"
                    IsEnabled="{Binding !IsProcessing}"
                    Classes="accent"/>
            <Button Name="ExportCsvButton" Content="导出CSV" 
                    Click="ExportCsvButton_Click"
                    Classes="outline"/>
            <Button Name="ClearButton" Content="清空结果" 
                    Click="ClearButton_Click"
                    Classes="outline"/>
        </StackPanel>

        <!-- 主内容区域 -->
        <Grid Grid.Row="1" Margin="10">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>

            <!-- 状态信息区域 -->
            <StackPanel Grid.Row="0" Margin="0,0,0,10">
                <!-- 主状态信息 -->
                <TextBlock Text="{Binding StatusMessage}" 
                           FontSize="14" 
                           Foreground="{DynamicResource SystemAccentColor}"/>
                
                <!-- 进度信息 -->
                <StackPanel Orientation="Horizontal" Margin="0,5,0,0" 
                            IsVisible="{Binding TotalFiles}">
                    <TextBlock Text="{Binding ProgressMessage}" 
                               FontSize="12" 
                               Foreground="{DynamicResource SystemBaseMediumColor}"/>
                    <TextBlock Text="{Binding CurrentProcessingFile, StringFormat=' | 当前: {0}'}" 
                               FontSize="12" 
                               Foreground="{DynamicResource SystemBaseMediumColor}"
                               Margin="10,0,0,0"/>
                </StackPanel>
                
                <!-- 处理速度 -->
                <TextBlock Text="{Binding ProcessingSpeed}" 
                           FontSize="11" 
                           Foreground="{DynamicResource SystemBaseMediumColor}"
                           Margin="0,2,0,0"
                           IsVisible="{Binding IsProcessing}"/>
                
                <!-- 进度条 -->
                <ProgressBar Value="{Binding ProcessedFiles}" 
                             Maximum="{Binding TotalFiles}"
                             IsVisible="{Binding TotalFiles}"
                             Margin="0,5,0,0"/>
            </StackPanel>

            <!-- 数据表格 -->
            <ListBox Grid.Row="1" 
                     ItemsSource="{Binding TagInfos}"
                     ScrollViewer.HorizontalScrollBarVisibility="Auto"
                     ScrollViewer.VerticalScrollBarVisibility="Auto">
                <ListBox.ItemTemplate>
                    <DataTemplate>
                        <Grid Margin="5">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="120"/>
                                <ColumnDefinition Width="80"/>
                                <ColumnDefinition Width="200"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <TextBlock Grid.Column="0" Text="{Binding Tag}" FontWeight="Bold"/>
                            <TextBlock Grid.Column="1" Text="{Binding Count}" HorizontalAlignment="Center"/>
                            <TextBlock Grid.Column="2" Text="{Binding SourceIds}" TextWrapping="Wrap" FontSize="11"/>
                            <TextBlock Grid.Column="3" Text="{Binding SourceFiles}" TextWrapping="Wrap" FontSize="11" 
                                       Foreground="{DynamicResource SystemBaseMediumColor}"/>
                        </Grid>
                    </DataTemplate>
                </ListBox.ItemTemplate>
            </ListBox>
        </Grid>

        <!-- 底部状态栏 -->
        <Border Grid.Row="2" Background="{DynamicResource SystemControlBackgroundBaseLowBrush}" 
                Padding="10,5">
            <TextBlock Text="开发者: jiashuai_xu@qq.com | 版本: 1.0.0 | 支持批量处理" 
                       HorizontalAlignment="Right"/>
        </Border>
    </Grid>
</Window>
```

#### 代码后台事件处理

```csharp
// MainWindow.axaml.cs - 批量处理事件处理
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
    }

    // 文件夹选择事件处理
    private async void SelectFolderButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var options = new FolderPickerOpenOptions
            {
                Title = "选择包含txt文件的文件夹",
                AllowMultiple = false
            };

            var folders = await StorageProvider.OpenFolderPickerAsync(options);
            
            if (folders.Count > 0)
            {
                var folder = folders[0];
                await _viewModel.ProcessFolderAsync(folder.Path.LocalPath);
            }
        }
        catch (Exception ex)
        {
            _viewModel.StatusMessage = $"文件夹处理失败：{ex.Message}";
        }
    }

    // 增强的CSV导出
    private async void ExportCsvButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var options = new FilePickerSaveOptions
            {
                Title = "保存CSV文件",
                DefaultExtension = "csv",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("CSV文件")
                    {
                        Patterns = new[] { "*.csv" }
                    }
                }
            };

            var file = await StorageProvider.SaveFilePickerAsync(options);
            
            if (file != null)
            {
                await using var writer = new StreamWriter(file.Path.LocalPath);
                await using var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
                
                // 写入标题 - 包含来源文件列
                csv.WriteField("标签");
                csv.WriteField("出现次数");
                csv.WriteField("来源ID");
                csv.WriteField("来源文件");
                await csv.NextRecordAsync();

                // 写入数据
                foreach (var tagInfo in _viewModel.TagInfos)
                {
                    csv.WriteField(tagInfo.Tag);
                    csv.WriteField(tagInfo.Count);
                    csv.WriteField(tagInfo.SourceIds);
                    csv.WriteField(tagInfo.SourceFiles);
                    await csv.NextRecordAsync();
                }

                _viewModel.StatusMessage = $"CSV文件已保存到：{file.Path.LocalPath}";
            }
        }
        catch (Exception ex)
        {
            _viewModel.StatusMessage = $"CSV导出失败：{ex.Message}";
        }
    }
}
```

### 4. Model 层扩展

#### 增强的 TagInfoViewModel

```csharp
// ViewModels/TagInfoViewModel.cs - 批量处理扩展
public class TagInfoViewModel : INotifyPropertyChanged
{
    private string _tag = string.Empty;
    private int _count;
    private string _sourceIds = string.Empty;
    private string _sourceFiles = string.Empty;

    public string Tag
    {
        get => _tag;
        set
        {
            _tag = value;
            OnPropertyChanged();
        }
    }

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            OnPropertyChanged();
        }
    }

    public string SourceIds
    {
        get => _sourceIds;
        set
        {
            _sourceIds = value;
            OnPropertyChanged();
        }
    }

    public string SourceFiles
    {
        get => _sourceFiles;
        set
        {
            _sourceFiles = value;
            OnPropertyChanged();
        }
    }

    // 计算属性
    public string DisplayText => $"{Tag}: {Count} 次";
    
    public bool HasMultipleFiles => !string.IsNullOrEmpty(_sourceFiles) && _sourceFiles.Contains(",");
    
    public int FileCount => string.IsNullOrEmpty(_sourceFiles) ? 0 : _sourceFiles.Split(',').Length;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### 5. 数据绑定最佳实践

#### 双向绑定示例

```xml
<!-- 双向绑定示例 -->
<TextBox Text="{Binding StatusMessage, Mode=TwoWay}" 
         IsReadOnly="True"/>

<!-- 条件绑定示例 -->
<StackPanel IsVisible="{Binding IsProcessing}">
    <ProgressBar Value="{Binding ProcessedFiles}" 
                  Maximum="{Binding TotalFiles}"/>
    <TextBlock Text="{Binding ProgressMessage}"/>
</StackPanel>

<!-- 集合绑定示例 -->
<ListBox ItemsSource="{Binding TagInfos}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel>
                <TextBlock Text="{Binding Tag}" FontWeight="Bold"/>
                <TextBlock Text="{Binding Count, StringFormat='出现 {0} 次'}"/>
                <TextBlock Text="{Binding SourceFiles, StringFormat='来源: {0}'}" 
                           FontSize="10" Foreground="Gray"/>
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### 命令绑定示例

```csharp
// ViewModel 中的命令实现
public class MainWindowViewModel : INotifyPropertyChanged
{
    private ICommand? _processFolderCommand;
    private ICommand? _exportCsvCommand;
    
    public ICommand ProcessFolderCommand => 
        _processFolderCommand ??= new AsyncRelayCommand<string>(ProcessFolderAsync);
    
    public ICommand ExportCsvCommand => 
        _exportCsvCommand ??= new RelayCommand(ExportCsv, CanExportCsv);
    
    private bool CanExportCsv() => TagInfos.Count > 0 && !IsProcessing;
    
    private void ExportCsv()
    {
        // 导出逻辑
    }
}
```

### 6. 测试策略

#### ViewModel 测试

```csharp
// ViewModel 单元测试
[TestClass]
public class MainWindowViewModelTests
{
    private MainWindowViewModel _viewModel = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _viewModel = new MainWindowViewModel();
    }
    
    [TestMethod]
    public async Task ProcessFolderAsync_ShouldUpdateProgressCorrectly()
    {
        // Arrange
        var testFolder = CreateTestFolder();
        
        // Act
        await _viewModel.ProcessFolderAsync(testFolder);
        
        // Assert
        Assert.IsTrue(_viewModel.TotalFiles > 0);
        Assert.AreEqual(_viewModel.TotalFiles, _viewModel.ProcessedFiles);
        Assert.IsFalse(_viewModel.IsProcessing);
        Assert.IsTrue(_viewModel.TagInfos.Count > 0);
        
        // Cleanup
        Directory.Delete(testFolder, true);
    }
    
    [TestMethod]
    public void ProgressMessage_ShouldCalculateCorrectly()
    {
        // Arrange
        _viewModel.TotalFiles = 10;
        _viewModel.ProcessedFiles = 3;
        
        // Act
        var progressMessage = _viewModel.ProgressMessage;
        
        // Assert
        Assert.AreEqual("进度: 3/10 (30.0%)", progressMessage);
    }
    
    private string CreateTestFolder()
    {
        var folder = Path.Combine(Path.GetTempPath(), "TestFolder");
        Directory.CreateDirectory(folder);
        
        File.WriteAllText(Path.Combine(folder, "test1.txt"), 
            "M35_230001【测试标签】");
        File.WriteAllText(Path.Combine(folder, "test2.txt"), 
            "M35_230002【另一个标签】");
        
        return folder;
    }
}
```

### 7. 性能优化

#### 内存优化

```csharp
// 内存优化的批量处理
public class MemoryOptimizedViewModel : MainWindowViewModel
{
    private const int MaxBatchSize = 100;
    
    public override async Task ProcessFolderAsync(string folderPath)
    {
        var files = await ScanTxtFilesAsync(folderPath);
        
        if (files.Length > MaxBatchSize)
        {
            await ProcessInBatches(files);
        }
        else
        {
            await ProcessDirectly(files);
        }
    }
    
    private async Task ProcessInBatches(string[] files)
    {
        var batchSize = MaxBatchSize;
        var allResults = new Dictionary<string, TagInfoViewModel>();
        
        for (int i = 0; i < files.Length; i += batchSize)
        {
            var batch = files.Skip(i).Take(batchSize);
            var batchResults = await ProcessBatch(batch);
            
            MergeBatchResults(allResults, batchResults);
            
            // 强制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        UpdateUIWithResults(allResults);
    }
}
```

### 8. 总结

批量处理功能的 MVVM 实现展现了以下特点：

1. **清晰的职责分离**：View 负责UI展示，ViewModel 负责业务逻辑，Model 负责数据处理
2. **强大的数据绑定**：实时更新进度、状态和结果
3. **异步处理模式**：不阻塞UI线程，提供良好的用户体验
4. **C# 14 现代语法**：集合表达式、模式匹配、计算属性等
5. **错误处理机制**：分层异常处理，优雅降级
6. **性能优化**：内存管理、批量处理、进度反馈

这套实现为处理大量文件提供了高效、稳定的 MVVM 解决方案！
