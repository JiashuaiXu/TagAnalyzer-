using System;
using System.IO;
using System.Linq;
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
        _viewModel = new MainWindowViewModel();
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
                var text = await File.ReadAllTextAsync(file.Path.LocalPath);
                _viewModel.ProcessText(text);
            }
        }
        catch (Exception ex)
        {
            _viewModel.StatusMessage = $"文件读取失败：{ex.Message}";
        }
    }

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

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.ClearResults();
    }

    private void AboutButton_Click(object? sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }
}
