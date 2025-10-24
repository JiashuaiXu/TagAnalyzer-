# Avalonia UI 语法详解

## 📋 目录

1. [XAML 基础语法](#xaml-基础语法)
2. [控件详解](#控件详解)
3. [布局系统](#布局系统)
4. [数据绑定](#数据绑定)
5. [样式和主题](#样式和主题)
6. [事件处理](#事件处理)
7. [资源管理](#资源管理)
8. [批量处理UI组件详解](#批量处理ui组件详解)

---

## 🎨 XAML 基础语法

### 1. 基本结构

```xml
<!-- 根元素定义 -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="MyApp.MainWindow"
        Title="我的应用">

    <!-- 内容 -->
</Window>
```

### 2. 命名空间

```xml
<!-- 常用命名空间 -->
xmlns="https://github.com/avaloniaui"                    <!-- Avalonia 核心 -->
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"   <!-- XAML 扩展 -->
xmlns:d="http://schemas.microsoft.com/expression/blend/2008"  <!-- 设计时 -->
xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"  <!-- 兼容性 -->
xmlns:vm="using:MyApp.ViewModels"                        <!-- 视图模型 -->
```

### 3. 属性语法

```xml
<!-- 属性赋值 -->
<Button Content="确定" Width="100" Height="30" />

<!-- 属性元素语法 -->
<Button>
    <Button.Content>确定</Button.Content>
    <Button.Width>100</Button.Width>
</Button>

<!-- 集合语法 -->
<StackPanel>
    <Button Content="按钮1"/>
    <Button Content="按钮2"/>
</StackPanel>
```

### 4. 标记扩展

```xml
<!-- 绑定扩展 -->
<TextBlock Text="{Binding StatusMessage}" />

<!-- 静态资源 -->
<Button Style="{StaticResource MyButtonStyle}" />

<!-- 动态资源 -->
<TextBlock Foreground="{DynamicResource SystemAccentColor}" />

<!-- 相对源绑定 -->
<Button Command="{Binding DataContext.SaveCommand, RelativeSource={RelativeSource AncestorType=Window}}" />
```

---

## 🎛️ 控件详解

### 1. 基础控件

#### Button (按钮)
```xml
<Button Content="确定" 
        Click="OnButtonClick"
        Classes="accent"
        IsEnabled="{Binding CanExecute}"/>
```

#### TextBlock (文本显示)
```xml
<TextBlock Text="静态文本" 
           FontSize="14" 
           FontWeight="Bold"
           Foreground="Red"
           TextWrapping="Wrap"/>
```

#### TextBox (文本输入)
```xml
<TextBox Text="{Binding UserInput}" 
         Watermark="请输入内容"
         MaxLength="100"/>
```

#### CheckBox (复选框)
```xml
<CheckBox Content="同意条款" 
          IsChecked="{Binding IsAgreed}"/>
```

#### ComboBox (下拉框)
```xml
<ComboBox ItemsSource="{Binding Options}"
          SelectedItem="{Binding SelectedOption}"
          DisplayMemberBinding="{Binding Name}"/>
```

### 2. 布局控件

#### Grid (网格)
```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="100"/>
    </Grid.RowDefinitions>
    
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    
    <Button Grid.Row="0" Grid.Column="0" Content="按钮"/>
    <TextBox Grid.Row="1" Grid.Column="1" Text="内容"/>
</Grid>
```

#### StackPanel (堆叠面板)
```xml
<StackPanel Orientation="Vertical" Spacing="10">
    <Button Content="按钮1"/>
    <Button Content="按钮2"/>
    <Button Content="按钮3"/>
</StackPanel>
```

#### DockPanel (停靠面板)
```xml
<DockPanel>
    <Button DockPanel.Dock="Top" Content="顶部"/>
    <Button DockPanel.Dock="Left" Content="左侧"/>
    <Button DockPanel.Dock="Right" Content="右侧"/>
    <Button Content="填充"/>
</DockPanel>
```

#### Canvas (画布)
```xml
<Canvas Width="400" Height="300">
    <Button Canvas.Left="50" Canvas.Top="50" Content="按钮"/>
    <Ellipse Canvas.Left="100" Canvas.Top="100" Width="50" Height="50" Fill="Red"/>
</Canvas>
```

### 3. 数据控件

#### ListBox (列表框)
```xml
<ListBox ItemsSource="{Binding Items}"
         SelectedItem="{Binding SelectedItem}"
         SelectionMode="Single">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Name}" FontWeight="Bold"/>
                <TextBlock Text="{Binding Description}" Margin="10,0"/>
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### DataGrid (数据表格)
```xml
<DataGrid ItemsSource="{Binding Data}"
          AutoGenerateColumns="False"
          IsReadOnly="True"
          GridLinesVisibility="Horizontal">
    <DataGrid.Columns>
        <DataGridTextColumn Header="名称" Binding="{Binding Name}" Width="150"/>
        <DataGridTextColumn Header="年龄" Binding="{Binding Age}" Width="100"/>
        <DataGridCheckBoxColumn Header="激活" Binding="{Binding IsActive}" Width="80"/>
    </DataGrid.Columns>
</DataGrid>
```

#### TreeView (树形视图)
```xml
<TreeView ItemsSource="{Binding TreeItems}">
    <TreeView.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Name}"/>
                <TextBlock Text="{Binding Count}" Margin="10,0"/>
            </StackPanel>
        </DataTemplate>
    </TreeView.ItemTemplate>
</TreeView>
```

---

## 📐 布局系统

### 1. 布局属性

#### 对齐属性
```xml
<Button HorizontalAlignment="Center" 
        VerticalAlignment="Top"
        Margin="10"/>
```

#### 边距和内边距
```xml
<!-- Margin: 外边距 -->
<Button Margin="10,5,10,5" Content="按钮"/>

<!-- Padding: 内边距 -->
<Border Padding="20" Background="LightGray">
    <TextBlock Text="内容"/>
</Border>
```

#### 尺寸属性
```xml
<Button Width="100" Height="30" Content="固定尺寸"/>
<Button MinWidth="50" MaxWidth="200" Content="范围尺寸"/>
<Button Width="*" Content="填充剩余空间"/>
```

### 2. 布局示例

#### 响应式布局
```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 标题栏 -->
        <RowDefinition Height="*"/>    <!-- 主内容 -->
        <RowDefinition Height="Auto"/>  <!-- 状态栏 -->
    </Grid.RowDefinitions>
    
    <!-- 标题栏 -->
    <Border Grid.Row="0" Background="DarkBlue" Padding="10">
        <TextBlock Text="应用标题" Foreground="White" FontSize="18"/>
    </Border>
    
    <!-- 主内容 -->
    <Grid Grid.Row="1" Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="200"/>  <!-- 侧边栏 -->
            <ColumnDefinition Width="*"/>     <!-- 主区域 -->
        </Grid.ColumnDefinitions>
        
        <ListBox Grid.Column="0" ItemsSource="{Binding MenuItems}"/>
        <ContentControl Grid.Column="1" Content="{Binding CurrentView}"/>
    </Grid>
    
    <!-- 状态栏 -->
    <Border Grid.Row="2" Background="LightGray" Padding="5">
        <TextBlock Text="{Binding StatusMessage}"/>
    </Border>
</Grid>
```

---

## 🔗 数据绑定

### 1. 绑定模式

```xml
<!-- 单向绑定 (默认) -->
<TextBlock Text="{Binding StatusMessage}"/>

<!-- 双向绑定 -->
<TextBox Text="{Binding UserInput, Mode=TwoWay}"/>

<!-- 一次性绑定 -->
<TextBlock Text="{Binding StaticValue, Mode=OneTime}"/>

<!-- 单向到源绑定 -->
<TextBlock Text="{Binding ComputedValue, Mode=OneWayToSource}"/>
```

### 2. 绑定路径

```xml
<!-- 简单属性 -->
<TextBlock Text="{Binding Name}"/>

<!-- 嵌套属性 -->
<TextBlock Text="{Binding User.Profile.Name}"/>

<!-- 集合索引 -->
<TextBlock Text="{Binding Items[0].Name}"/>

<!-- 附加属性 -->
<Button Grid.Row="{Binding RowIndex}"/>
```

### 3. 绑定转换器

```xml
<!-- 使用转换器 -->
<TextBlock Text="{Binding IsActive, Converter={StaticResource BoolToStringConverter}}"/>

<!-- 转换器参数 -->
<TextBlock Foreground="{Binding Status, Converter={StaticResource StatusToColorConverter}, ConverterParameter=Warning}"/>
```

### 4. 绑定验证

```xml
<TextBox Text="{Binding Age, Mode=TwoWay, NotifyOnValidationError=True}">
    <TextBox.Text>
        <Binding Path="Age" Mode="TwoWay">
            <Binding.ValidationRules>
                <DataAnnotationsValidationRule/>
            </Binding.ValidationRules>
        </Binding>
    </TextBox.Text>
</TextBox>
```

---

## 🎨 样式和主题

### 1. 内联样式

```xml
<Button Background="Blue" 
        Foreground="White" 
        FontSize="16" 
        FontWeight="Bold"
        Padding="10,5"/>
```

### 2. 样式资源

```xml
<!-- 定义样式 -->
<Window.Resources>
    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="Blue"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="FontSize" Value="16"/>
        <Setter Property="Padding" Value="10,5"/>
    </Style>
</Window.Resources>

<!-- 使用样式 -->
<Button Style="{StaticResource PrimaryButtonStyle}" Content="确定"/>
```

### 3. 样式继承

```xml
<Style x:Key="BaseButtonStyle" TargetType="Button">
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="Padding" Value="8,4"/>
</Style>

<Style x:Key="AccentButtonStyle" TargetType="Button" BasedOn="{StaticResource BaseButtonStyle}">
    <Setter Property="Background" Value="Orange"/>
    <Setter Property="Foreground" Value="White"/>
</Style>
```

### 4. 触发器

```xml
<Style TargetType="Button">
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="LightBlue"/>
        </Trigger>
        <Trigger Property="IsPressed" Value="True">
            <Setter Property="Background" Value="DarkBlue"/>
        </Trigger>
    </Style.Triggers>
</Style>
```

### 5. 主题系统

```xml
<!-- 应用主题 -->
<Application.Styles>
    <FluentTheme Mode="Light"/>
    <!-- 或 -->
    <FluentTheme Mode="Dark"/>
    <!-- 或 -->
    <FluentTheme Mode="Auto"/>
</Application.Styles>
```

---

## ⚡ 事件处理

### 1. 代码后台事件

```xml
<!-- XAML -->
<Button Click="OnButtonClick" Content="点击我"/>
```

```csharp
// 代码后台
private void OnButtonClick(object? sender, RoutedEventArgs e)
{
    var button = sender as Button;
    MessageBox.Show($"按钮被点击了: {button?.Content}");
}
```

### 2. 命令绑定

```xml
<!-- XAML -->
<Button Command="{Binding SaveCommand}" Content="保存"/>
```

```csharp
// ViewModel
public class MainViewModel
{
    public ICommand SaveCommand { get; }
    
    public MainViewModel()
    {
        SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
    }
    
    private void ExecuteSave()
    {
        // 保存逻辑
    }
    
    private bool CanExecuteSave()
    {
        return !string.IsNullOrEmpty(FileName);
    }
}
```

### 3. 事件路由

```xml
<!-- 冒泡事件 -->
<StackPanel Button.Click="OnAnyButtonClick">
    <Button Content="按钮1"/>
    <Button Content="按钮2"/>
</StackPanel>
```

```csharp
private void OnAnyButtonClick(object? sender, RoutedEventArgs e)
{
    // 处理任何子按钮的点击事件
}
```

---

## 📦 资源管理

### 1. 本地资源

```xml
<Window.Resources>
    <SolidColorBrush x:Key="PrimaryBrush" Color="Blue"/>
    <Style x:Key="MyStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
    </Style>
</Window.Resources>
```

### 2. 资源字典

```xml
<!-- Resources.xaml -->
<ResourceDictionary xmlns="https://github.com/avaloniaui">
    <SolidColorBrush x:Key="AccentBrush" Color="Orange"/>
    <Style x:Key="AccentButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="{StaticResource AccentBrush}"/>
    </Style>
</ResourceDictionary>
```

```xml
<!-- 引用资源字典 -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### 3. 动态资源

```xml
<!-- 支持运行时主题切换 -->
<TextBlock Foreground="{DynamicResource SystemAccentColor}"/>
<Button Background="{DynamicResource SystemControlBackgroundBrush}"/>
```

---

## 🔧 高级特性

### 1. 自定义控件

```csharp
// 自定义控件
public class CustomButton : Button
{
    public static readonly StyledProperty<string> CustomTextProperty =
        AvaloniaProperty.Register<CustomButton, string>(nameof(CustomText));

    public string CustomText
    {
        get => GetValue(CustomTextProperty);
        set => SetValue(CustomTextProperty, value);
    }
}
```

```xml
<!-- 使用自定义控件 -->
<local:CustomButton CustomText="自定义文本" Content="自定义按钮"/>
```

### 2. 附加属性

```csharp
// 定义附加属性
public class GridExtensions
{
    public static readonly AttachedProperty<int> RowSpanProperty =
        AvaloniaProperty.RegisterAttached<GridExtensions, Control, int>("RowSpan");

    public static int GetRowSpan(Control element)
    {
        return element.GetValue(RowSpanProperty);
    }

    public static void SetRowSpan(Control element, int value)
    {
        element.SetValue(RowSpanProperty, value);
    }
}
```

### 3. 行为 (Behaviors)

```csharp
// 定义行为
public class FocusBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.GotFocus += OnGotFocus;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.GotFocus -= OnGotFocus;
        base.OnDetaching();
    }

    private void OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        AssociatedObject.SelectAll();
    }
}
```

---

## 📚 最佳实践

### 1. XAML 组织

```xml
<!-- 好的结构 -->
<Window>
    <Window.Resources>
        <!-- 资源定义 -->
    </Window.Resources>
    
    <Grid>
        <!-- 布局结构 -->
    </Grid>
</Window>
```

### 2. 命名约定

```xml
<!-- 控件命名 -->
<Button x:Name="SaveButton" Content="保存"/>
<TextBox x:Name="UserNameTextBox" Text="{Binding UserName}"/>
```

### 3. 性能优化

```xml
<!-- 使用虚拟化 -->
<ListBox VirtualizationMode="Recycling" 
         ItemsSource="{Binding LargeCollection}"/>

<!-- 延迟加载 -->
<ContentControl Content="{Binding LazyContent}"/>
```

### 4. 可访问性

```xml
<!-- 添加可访问性属性 -->
<Button Content="保存" 
        AutomationProperties.Name="保存文件"
        AutomationProperties.HelpText="点击保存当前文档"/>
```

---

## 🎯 总结

Avalonia UI 的 XAML 语法提供了：

1. **丰富的控件库**：满足各种 UI 需求
2. **强大的布局系统**：灵活的界面设计
3. **完善的数据绑定**：MVVM 模式支持
4. **灵活的样式系统**：主题和样式管理
5. **事件处理机制**：用户交互支持
6. **资源管理**：代码重用和维护

掌握这些语法特性，可以创建出功能丰富、界面美观的跨平台桌面应用程序！

---

## 🚀 批量处理UI组件详解

### 1. 进度条 (ProgressBar) 组件

#### 基础进度条实现

```xml
<!-- 基础进度条 -->
<ProgressBar Value="{Binding ProcessedFiles}" 
             Maximum="{Binding TotalFiles}"
             IsVisible="{Binding TotalFiles}"
             Margin="0,5,0,0"/>

<!-- 带样式的进度条 -->
<ProgressBar Value="{Binding ProcessedFiles}" 
             Maximum="{Binding TotalFiles}"
             IsVisible="{Binding TotalFiles}"
             Margin="0,5,0,0">
    <ProgressBar.Styles>
        <Style Selector="ProgressBar">
            <Setter Property="Background" Value="{DynamicResource SystemControlBackgroundBaseLowBrush}"/>
            <Setter Property="Foreground" Value="{DynamicResource SystemAccentColor}"/>
            <Setter Property="Height" Value="8"/>
            <Setter Property="CornerRadius" Value="4"/>
        </Style>
    </ProgressBar.Styles>
</ProgressBar>
```

#### 进度条属性详解

```xml
<!-- 进度条完整配置 -->
<ProgressBar Value="{Binding ProcessedFiles}"           <!-- 当前值 -->
             Maximum="{Binding TotalFiles}"             <!-- 最大值 -->
             Minimum="0"                               <!-- 最小值 -->
             IsVisible="{Binding TotalFiles}"           <!-- 可见性 -->
             IsIndeterminate="False"                    <!-- 是否不确定进度 -->
             ShowProgressText="True"                    <!-- 显示进度文本 -->
             ProgressTextFormat="{}{0:P0}"              <!-- 进度文本格式 -->
             Margin="0,5,0,0"                          <!-- 边距 -->
             HorizontalAlignment="Stretch"             <!-- 水平对齐 -->
             VerticalAlignment="Center"/>              <!-- 垂直对齐 -->
```

#### 自定义进度条样式

```xml
<!-- 自定义进度条样式 -->
<ProgressBar Value="{Binding ProcessedFiles}" 
             Maximum="{Binding TotalFiles}"
             Classes="CustomProgressBar">
    <ProgressBar.Styles>
        <Style Selector="ProgressBar.CustomProgressBar">
            <!-- 背景样式 -->
            <Setter Property="Background" Value="#E0E0E0"/>
            <Setter Property="Height" Value="12"/>
            <Setter Property="CornerRadius" Value="6"/>
            
            <!-- 进度条填充样式 -->
            <Setter Property="Template">
                <ControlTemplate>
                    <Border Background="{TemplateBinding Background}"
                            CornerRadius="{TemplateBinding CornerRadius}">
                        <Border Name="PART_Indicator"
                                Background="{TemplateBinding Foreground}"
                                CornerRadius="{TemplateBinding CornerRadius}"
                                HorizontalAlignment="Left"
                                Width="{TemplateBinding Value, Converter={x:Static Converters.Percentage}}"/>
                    </Border>
                </ControlTemplate>
            </Setter>
        </Style>
    </ProgressBar.Styles>
</ProgressBar>
```

### 2. 文件夹选择对话框

#### FolderPickerOpenOptions 配置

```csharp
// 文件夹选择对话框配置
private async void SelectFolderButton_Click(object? sender, RoutedEventArgs e)
{
    try
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "选择包含txt文件的文件夹",           // 对话框标题
            AllowMultiple = false,                      // 是否允许多选
            SuggestedStartLocation = await StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents) // 建议起始位置
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
```

#### 高级文件夹选择配置

```csharp
// 高级文件夹选择配置
private async Task<string?> SelectFolderWithAdvancedOptions()
{
    try
    {
        var options = new FolderPickerOpenOptions
        {
            Title = "选择批量处理文件夹",
            AllowMultiple = false,
            SuggestedStartLocation = await StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents),
            
            // 自定义文件类型过滤器（虽然文件夹选择器通常不需要）
            FileTypeFilter = new[]
            {
                new FilePickerFileType("所有文件夹")
                {
                    Patterns = new[] { "*" }
                }
            }
        };

        var folders = await StorageProvider.OpenFolderPickerAsync(options);
        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }
    catch (Exception ex)
    {
        _viewModel.StatusMessage = $"文件夹选择失败：{ex.Message}";
        return null;
    }
}
```

### 3. 状态信息显示组件

#### 多层级状态信息布局

```xml
<!-- 状态信息区域 -->
<StackPanel Grid.Row="0" Margin="0,0,0,10">
    <!-- 主状态信息 -->
    <TextBlock Text="{Binding StatusMessage}" 
               FontSize="14" 
               FontWeight="SemiBold"
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
    
    <!-- 处理速度信息 -->
    <TextBlock Text="{Binding ProcessingSpeed}" 
               FontSize="11" 
               Foreground="{DynamicResource SystemBaseMediumColor}"
               Margin="0,2,0,0"
               IsVisible="{Binding IsProcessing}"/>
    
    <!-- 预计剩余时间 -->
    <TextBlock Text="{Binding EstimatedTimeRemaining, StringFormat='预计剩余时间: {0:mm\\:ss}'}" 
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
```

#### 状态信息动画效果

```xml
<!-- 带动画的状态信息 -->
<StackPanel Grid.Row="0" Margin="0,0,0,10">
    <!-- 主状态信息 - 带淡入动画 -->
    <TextBlock Text="{Binding StatusMessage}" 
               FontSize="14" 
               FontWeight="SemiBold"
               Foreground="{DynamicResource SystemAccentColor}">
        <TextBlock.Styles>
            <Style Selector="TextBlock">
                <Setter Property="Opacity" Value="0"/>
                <Style.Animations>
                    <Animation Duration="0:0:0.3">
                        <KeyFrame Cue="0%" Property="Opacity" Value="0"/>
                        <KeyFrame Cue="100%" Property="Opacity" Value="1"/>
                    </Animation>
                </Style.Animations>
            </Style>
        </TextBlock.Styles>
    </TextBlock>
    
    <!-- 进度条 - 带进度动画 -->
    <ProgressBar Value="{Binding ProcessedFiles}" 
                 Maximum="{Binding TotalFiles}"
                 IsVisible="{Binding TotalFiles}"
                 Margin="0,5,0,0">
        <ProgressBar.Styles>
            <Style Selector="ProgressBar">
                <Setter Property="Opacity" Value="0"/>
                <Style.Animations>
                    <Animation Duration="0:0:0.5">
                        <KeyFrame Cue="0%" Property="Opacity" Value="0"/>
                        <KeyFrame Cue="100%" Property="Opacity" Value="1"/>
                    </Animation>
                </Style.Animations>
            </Style>
        </ProgressBar.Styles>
    </ProgressBar>
</StackPanel>
```

### 4. 增强的数据表格显示

#### 多列数据表格布局

```xml
<!-- 增强的数据表格 -->
<ListBox Grid.Row="1" 
         ItemsSource="{Binding TagInfos}"
         ScrollViewer.HorizontalScrollBarVisibility="Auto"
         ScrollViewer.VerticalScrollBarVisibility="Auto">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Grid Margin="5">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="120"/>    <!-- 标签列 -->
                    <ColumnDefinition Width="80"/>     <!-- 计数列 -->
                    <ColumnDefinition Width="200"/>    <!-- 来源ID列 -->
                    <ColumnDefinition Width="*"/>       <!-- 来源文件列 -->
                </Grid.ColumnDefinitions>
                
                <!-- 标签列 -->
                <TextBlock Grid.Column="0" 
                           Text="{Binding Tag}" 
                           FontWeight="Bold"
                           VerticalAlignment="Center"/>
                
                <!-- 计数列 -->
                <TextBlock Grid.Column="1" 
                           Text="{Binding Count}" 
                           HorizontalAlignment="Center"
                           VerticalAlignment="Center"
                           FontSize="12"
                           Foreground="{DynamicResource SystemAccentColor}"/>
                
                <!-- 来源ID列 -->
                <TextBlock Grid.Column="2" 
                           Text="{Binding SourceIds}" 
                           TextWrapping="Wrap" 
                           FontSize="11"
                           VerticalAlignment="Center"/>
                
                <!-- 来源文件列 -->
                <TextBlock Grid.Column="3" 
                           Text="{Binding SourceFiles}" 
                           TextWrapping="Wrap" 
                           FontSize="11" 
                           Foreground="{DynamicResource SystemBaseMediumColor}"
                           VerticalAlignment="Center"/>
            </Grid>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### 表格头部和样式

```xml
<!-- 带表头的数据表格 -->
<Grid Grid.Row="1">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 表头 -->
        <RowDefinition Height="*"/>     <!-- 数据行 -->
    </Grid.RowDefinitions>
    
    <!-- 表头 -->
    <Border Grid.Row="0" 
            Background="{DynamicResource SystemControlBackgroundBaseLowBrush}"
            Padding="5"
            CornerRadius="4,4,0,0">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="120"/>
                <ColumnDefinition Width="80"/>
                <ColumnDefinition Width="200"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            
            <TextBlock Grid.Column="0" Text="标签" FontWeight="Bold" FontSize="12"/>
            <TextBlock Grid.Column="1" Text="次数" FontWeight="Bold" FontSize="12" HorizontalAlignment="Center"/>
            <TextBlock Grid.Column="2" Text="来源ID" FontWeight="Bold" FontSize="12"/>
            <TextBlock Grid.Column="3" Text="来源文件" FontWeight="Bold" FontSize="12"/>
        </Grid>
    </Border>
    
    <!-- 数据行 -->
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
```

### 5. 工具栏按钮组

#### 响应式工具栏布局

```xml
<!-- 响应式工具栏 -->
<StackPanel Grid.Row="0" Orientation="Horizontal" Margin="10" Spacing="10">
    <!-- 主要操作按钮 -->
    <Button Name="SelectFileButton" 
            Content="选择文件" 
            Click="SelectFileButton_Click"
            IsEnabled="{Binding !IsProcessing}"
            Classes="accent"
            MinWidth="100"/>
            
    <Button Name="SelectFolderButton" 
            Content="选择文件夹" 
            Click="SelectFolderButton_Click"
            IsEnabled="{Binding !IsProcessing}"
            Classes="accent"
            MinWidth="100"/>
    
    <!-- 分隔符 -->
    <Separator Margin="10,0"/>
    
    <!-- 次要操作按钮 -->
    <Button Name="ExportCsvButton" 
            Content="导出CSV" 
            Click="ExportCsvButton_Click"
            Classes="outline"
            MinWidth="80"/>
            
    <Button Name="ClearButton" 
            Content="清空结果" 
            Click="ClearButton_Click"
            Classes="outline"
            MinWidth="80"/>
</StackPanel>
```

#### 按钮样式和状态

```xml
<!-- 自定义按钮样式 -->
<Button Name="SelectFolderButton" 
        Content="选择文件夹" 
        Click="SelectFolderButton_Click"
        IsEnabled="{Binding !IsProcessing}"
        Classes="accent">
    <Button.Styles>
        <Style Selector="Button.accent">
            <Setter Property="Background" Value="{DynamicResource SystemAccentColor}"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="Padding" Value="12,8"/>
            <Setter Property="CornerRadius" Value="4"/>
            <Setter Property="FontWeight" Value="SemiBold"/>
            
            <!-- 悬停效果 -->
            <Style.Animations>
                <Animation Duration="0:0:0.2">
                    <KeyFrame Cue="0%" Property="Opacity" Value="1"/>
                    <KeyFrame Cue="100%" Property="Opacity" Value="0.8"/>
                </Animation>
            </Style.Animations>
        </Style>
        
        <!-- 禁用状态样式 -->
        <Style Selector="Button.accent:disabled">
            <Setter Property="Background" Value="{DynamicResource SystemControlBackgroundBaseLowBrush}"/>
            <Setter Property="Foreground" Value="{DynamicResource SystemBaseMediumColor}"/>
            <Setter Property="Opacity" Value="0.5"/>
        </Style>
    </Button.Styles>
</Button>
```

### 6. 响应式布局设计

#### 自适应窗口大小

```xml
<!-- 响应式主窗口布局 -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:TagAnalyzer.ViewModels"
        x:Class="TagAnalyzer.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Title="标签分析工具 - 批量处理版 - jiashuai_xu@qq.com"
        Width="1000" Height="800"
        MinWidth="800" MinHeight="600"
        MaxWidth="1600" MaxHeight="1200">

    <Grid RowDefinitions="Auto,*,Auto">
        <!-- 工具栏 - 固定高度 -->
        <StackPanel Grid.Row="0" 
                    Orientation="Horizontal" 
                    Margin="10" 
                    Spacing="10"
                    Height="50">
            <!-- 按钮内容 -->
        </StackPanel>

        <!-- 主内容区域 - 自适应 -->
        <Grid Grid.Row="1" Margin="10">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>  <!-- 状态信息 - 自适应 -->
                <RowDefinition Height="*"/>     <!-- 数据表格 - 填充剩余空间 -->
            </Grid.RowDefinitions>
            
            <!-- 状态信息区域 -->
            <StackPanel Grid.Row="0" Margin="0,0,0,10">
                <!-- 状态信息内容 -->
            </StackPanel>

            <!-- 数据表格区域 -->
            <ListBox Grid.Row="1" 
                     ItemsSource="{Binding TagInfos}"
                     ScrollViewer.HorizontalScrollBarVisibility="Auto"
                     ScrollViewer.VerticalScrollBarVisibility="Auto">
                <!-- 表格内容 -->
            </ListBox>
        </Grid>

        <!-- 底部状态栏 - 固定高度 -->
        <Border Grid.Row="2" 
                Background="{DynamicResource SystemControlBackgroundBaseLowBrush}" 
                Padding="10,5"
                Height="30">
            <TextBlock Text="开发者: jiashuai_xu@qq.com | 版本: 1.0.0 | 支持批量处理" 
                       HorizontalAlignment="Right"/>
        </Border>
    </Grid>
</Window>
```

### 7. 数据绑定最佳实践

#### 复杂数据绑定示例

```xml
<!-- 复杂数据绑定 -->
<StackPanel>
    <!-- 条件显示 -->
    <StackPanel IsVisible="{Binding IsProcessing}">
        <TextBlock Text="正在处理中..." FontWeight="Bold"/>
        <ProgressBar Value="{Binding ProcessedFiles}" 
                     Maximum="{Binding TotalFiles}"/>
    </StackPanel>
    
    <!-- 格式化绑定 -->
    <TextBlock Text="{Binding ProcessedFiles, StringFormat='已处理 {0} 个文件'}"/>
    <TextBlock Text="{Binding TotalFiles, StringFormat='共 {0} 个文件'}"/>
    
    <!-- 计算属性绑定 -->
    <TextBlock Text="{Binding ProgressMessage}"/>
    
    <!-- 集合绑定 -->
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
</StackPanel>
```

### 8. 性能优化UI技巧

#### 虚拟化列表

```xml
<!-- 使用虚拟化提高性能 -->
<ListBox ItemsSource="{Binding TagInfos}"
         VirtualizationMode="Recycling"
         ScrollViewer.HorizontalScrollBarVisibility="Auto"
         ScrollViewer.VerticalScrollBarVisibility="Auto">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <!-- 简化的模板以提高性能 -->
            <Grid Margin="2">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="120"/>
                    <ColumnDefinition Width="80"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                
                <TextBlock Grid.Column="0" Text="{Binding Tag}" FontWeight="Bold"/>
                <TextBlock Grid.Column="1" Text="{Binding Count}" HorizontalAlignment="Center"/>
                <TextBlock Grid.Column="2" Text="{Binding SourceFiles}" TextWrapping="Wrap" FontSize="11"/>
            </Grid>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### 延迟加载和分页

```xml
<!-- 分页显示大量数据 -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    
    <!-- 数据列表 -->
    <ListBox Grid.Row="0" 
             ItemsSource="{Binding CurrentPageItems}"
             VirtualizationMode="Recycling">
        <!-- 列表内容 -->
    </ListBox>
    
    <!-- 分页控件 -->
    <StackPanel Grid.Row="1" 
                Orientation="Horizontal" 
                HorizontalAlignment="Center"
                Margin="0,10,0,0">
        <Button Content="上一页" 
                Click="PreviousPage_Click"
                IsEnabled="{Binding CanGoPrevious}"/>
        <TextBlock Text="{Binding PageInfo}" 
                   Margin="10,0"
                   VerticalAlignment="Center"/>
        <Button Content="下一页" 
                Click="NextPage_Click"
                IsEnabled="{Binding CanGoNext}"/>
    </StackPanel>
</Grid>
```

### 9. 总结

批量处理UI组件的设计要点：

1. **进度反馈**：使用 ProgressBar 提供直观的处理进度
2. **状态显示**：多层级状态信息，包括当前文件、处理速度等
3. **文件夹选择**：使用 FolderPickerOpenOptions 提供用户友好的文件夹选择
4. **数据表格**：多列显示，包含标签、计数、来源ID、来源文件
5. **响应式布局**：自适应窗口大小，提供良好的用户体验
6. **性能优化**：虚拟化列表、分页显示等性能优化技巧
7. **样式美化**：自定义样式、动画效果提升视觉效果

这些UI组件为批量处理功能提供了完整、美观、高效的用户界面！
