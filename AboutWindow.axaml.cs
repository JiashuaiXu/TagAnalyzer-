using Avalonia.Controls;
using Avalonia.Interactivity;
using TagAnalyzer.Models;

namespace TagAnalyzer;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        
        // 使用 C# 原生方式设置版本信息
        VersionTextBlock.Text = $"版本 {VersionInfo.ShortVersion}";
        VersionInfoTextBlock.Text = VersionInfo.GetFullVersionInfo();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

