using System.Reflection;

namespace TagAnalyzer.Models;

/// <summary>
/// 版本信息工具类，使用 C# 原生反射获取程序集版本信息
/// </summary>
public static class VersionInfo
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    
    /// <summary>
    /// 获取程序集版本号（AssemblyVersion）
    /// </summary>
    public static string AssemblyVersion
    {
        get
        {
            var version = Assembly.GetName().Version;
            return version != null ? $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}" : "Unknown";
        }
    }
    
    /// <summary>
    /// 获取简化版本号（主版本.次版本）
    /// </summary>
    public static string ShortVersion
    {
        get
        {
            var version = Assembly.GetName().Version;
            return version != null ? $"{version.Major}.{version.Minor}" : "Unknown";
        }
    }
    
    /// <summary>
    /// 获取程序集标题（AssemblyTitle）
    /// </summary>
    public static string Title
    {
        get
        {
            var attribute = Assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            return attribute?.Title ?? "Tag Analyzer";
        }
    }
    
    /// <summary>
    /// 获取程序集描述（AssemblyDescription）
    /// </summary>
    public static string Description
    {
        get
        {
            var attribute = Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            return attribute?.Description ?? string.Empty;
        }
    }
    
    /// <summary>
    /// 获取公司信息（AssemblyCompany）
    /// </summary>
    public static string Company
    {
        get
        {
            var attribute = Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return attribute?.Company ?? string.Empty;
        }
    }
    
    /// <summary>
    /// 获取产品信息（AssemblyProduct）
    /// </summary>
    public static string Product
    {
        get
        {
            var attribute = Assembly.GetCustomAttribute<AssemblyProductAttribute>();
            return attribute?.Product ?? "Tag Analyzer";
        }
    }
    
    /// <summary>
    /// 获取版权信息（AssemblyCopyright）
    /// </summary>
    public static string Copyright
    {
        get
        {
            var attribute = Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            return attribute?.Copyright ?? string.Empty;
        }
    }
    
    /// <summary>
    /// 获取 .NET 运行时版本
    /// </summary>
    public static string RuntimeVersion => Environment.Version.ToString();
    
    /// <summary>
    /// 获取目标框架
    /// </summary>
    public static string TargetFramework
    {
        get
        {
            try
            {
                var frameworkAttribute = Assembly.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();
                if (frameworkAttribute != null)
                {
                    // 提取框架名称，例如 "NET 10.0" 或 ".NETCoreApp,Version=v10.0"
                    var frameworkName = frameworkAttribute.FrameworkName;
                    if (frameworkName.Contains("Version=v"))
                    {
                        var version = frameworkName.Split("Version=v")[1].Split(',')[0];
                        return $".NET {version}";
                    }
                    return frameworkName;
                }
            }
            catch
            {
                // 忽略异常
            }
            return ".NET 10.0";
        }
    }
    
    /// <summary>
    /// 获取完整的版本信息字符串
    /// </summary>
    public static string GetFullVersionInfo()
    {
        return $"""
            {Title}
            版本：{ShortVersion} ({AssemblyVersion})
            
            {Description}
            
            公司：{Company}
            产品：{Product}
            {Copyright}
            
            .NET 运行时：{RuntimeVersion}
            目标框架：{TargetFramework}
            """;
    }
}

