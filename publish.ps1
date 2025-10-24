# Tag Analyzer 发布脚本 (.NET 10 + C# 14)
# 作者：jiashuai_xu@qq.com

Write-Host "=== Tag Analyzer 发布脚本 (.NET 10 + C# 14) ===" -ForegroundColor Green
Write-Host "作者：jiashuai_xu@qq.com" -ForegroundColor Cyan
Write-Host ""

# 检查 .NET SDK
Write-Host "检查 .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "错误：未找到 .NET SDK，请先安装 .NET 10.0 SDK" -ForegroundColor Red
    exit 1
}
Write-Host "找到 .NET SDK 版本：$dotnetVersion" -ForegroundColor Green

# 检查是否支持 .NET 10
if ($dotnetVersion -notmatch "^10\.") {
    Write-Host "警告：当前 SDK 版本可能不支持 .NET 10，建议升级到 .NET 10 SDK" -ForegroundColor Yellow
}

# 清理之前的构建
Write-Host "清理之前的构建..." -ForegroundColor Yellow
dotnet clean --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "清理失败" -ForegroundColor Red
    exit 1
}

# 还原包
Write-Host "还原 NuGet 包..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "包还原失败" -ForegroundColor Red
    exit 1
}

# 发布为单文件
Write-Host "发布为单文件可执行程序..." -ForegroundColor Yellow
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output ./publish --property:PublishSingleFile=true --property:IncludeNativeLibrariesForSelfExtract=true
if ($LASTEXITCODE -ne 0) {
    Write-Host "发布失败" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== 发布完成 ===" -ForegroundColor Green
Write-Host "可执行文件位置：./publish/TagAnalyzer.exe" -ForegroundColor Cyan
Write-Host "文件大小：$((Get-Item ./publish/TagAnalyzer.exe).Length / 1MB) MB" -ForegroundColor Cyan
Write-Host ""
Write-Host "使用方法：" -ForegroundColor Yellow
Write-Host "1. 双击 TagAnalyzer.exe 运行程序" -ForegroundColor White
Write-Host "2. 点击'选择文件'按钮选择 .txt 文件" -ForegroundColor White
Write-Host "3. 程序会自动分析并显示结果" -ForegroundColor White
Write-Host "4. 点击'导出CSV'可以保存结果到文件" -ForegroundColor White
Write-Host ""
Write-Host "作者：jiashuai_xu@qq.com" -ForegroundColor Cyan
