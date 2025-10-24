using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagAnalyzer.Models;

namespace TagAnalyzer.ViewModels;

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ObservableCollection<TagInfoViewModel> _tagInfos = new();
    private string _statusMessage = "请选择文件或文件夹进行分析";
    private bool _isProcessing = false;
    private int _processedFiles = 0;
    private int _totalFiles = 0;
    private string _currentProcessingFile = string.Empty;

    public ObservableCollection<TagInfoViewModel> TagInfos
    {
        get => _tagInfos;
        set
        {
            _tagInfos = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            _isProcessing = value;
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
            OnPropertyChanged(nameof(ProgressMessage));
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

    public string ProgressMessage
    {
        get
        {
            if (_totalFiles > 0)
            {
                return $"进度: {_processedFiles}/{_totalFiles} ({_processedFiles * 100.0 / _totalFiles:F1}%)";
            }
            return string.Empty;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ProcessText(string text)
    {
        IsProcessing = true;
        StatusMessage = "正在分析文本...";

        try
        {
            var results = TextParser.ParseText(text);
            TagInfos.Clear();
            
            foreach (var result in results)
            {
                var viewModel = new TagInfoViewModel
                {
                    Tag = result.Tag,
                    Count = result.Count,
                    SourceIds = string.Join(", ", result.SourceIds)
                };
                TagInfos.Add(viewModel);
            }

            StatusMessage = $"分析完成，共找到 {results.Count} 个不同的标签";
        }
        catch (Exception ex)
        {
            StatusMessage = $"分析失败：{ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    public async Task ProcessFolderAsync(string folderPath)
    {
        IsProcessing = true;
        StatusMessage = "正在扫描文件夹...";
        ProcessedFiles = 0;
        TotalFiles = 0;

        try
        {
            // 获取所有txt文件
            var txtFiles = Directory.GetFiles(folderPath, "*.txt", SearchOption.AllDirectories);
            TotalFiles = txtFiles.Length;

            if (TotalFiles == 0)
            {
                StatusMessage = "文件夹中没有找到txt文件";
                return;
            }

            StatusMessage = $"找到 {TotalFiles} 个txt文件，开始批量处理...";

            // 合并所有结果
            var allTagInfos = new Dictionary<string, TagInfoViewModel>();

            foreach (var filePath in txtFiles)
            {
                try
                {
                    CurrentProcessingFile = Path.GetFileName(filePath);
                    StatusMessage = $"正在处理: {CurrentProcessingFile}";

                    var content = await File.ReadAllTextAsync(filePath);
                    var results = TextParser.ParseText(content);

                    // 合并结果
                    foreach (var result in results)
                    {
                        if (!allTagInfos.ContainsKey(result.Tag))
                        {
                            allTagInfos[result.Tag] = new TagInfoViewModel
                            {
                                Tag = result.Tag,
                                Count = 0,
                                SourceIds = string.Empty,
                                SourceFiles = string.Empty
                            };
                        }

                        allTagInfos[result.Tag].Count += result.Count;
                        
                        // 合并来源ID
                        var existingIds = allTagInfos[result.Tag].SourceIds.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList();
                        existingIds.AddRange(result.SourceIds);
                        allTagInfos[result.Tag].SourceIds = string.Join(", ", existingIds.Distinct());

                        // 添加文件信息
                        var fileName = Path.GetFileName(filePath);
                        if (!string.IsNullOrEmpty(allTagInfos[result.Tag].SourceFiles))
                        {
                            allTagInfos[result.Tag].SourceFiles += $", {fileName}";
                        }
                        else
                        {
                            allTagInfos[result.Tag].SourceFiles = fileName;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"处理文件 {Path.GetFileName(filePath)} 失败: {ex.Message}";
                }

                ProcessedFiles++;
            }

            // 更新UI
            TagInfos.Clear();
            foreach (var tagInfo in allTagInfos.Values.OrderBy(t => t.Tag))
            {
                TagInfos.Add(tagInfo);
            }

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

    public void ClearResults()
    {
        TagInfos.Clear();
        ProcessedFiles = 0;
        TotalFiles = 0;
        CurrentProcessingFile = string.Empty;
        StatusMessage = "请选择文件或文件夹进行分析";
    }
}
